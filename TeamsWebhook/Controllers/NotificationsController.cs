using System;
using System.Threading;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TeamsWebhook.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using Microsoft.Graph;
using System.Net.Http.Formatting;

using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;

namespace TeamsWebhook.Controllers
{

  [Route("api/[controller]")]
  [ApiController]
  public class NotificationsController : ControllerBase
  {
    //private readonly MyConfig config;
    private readonly IConfiguration Configuration;
    private readonly ILogger<NotificationsController> _logger;
    private static Dictionary<string, Subscription> Subscriptions = new Dictionary<string, Subscription>();
    private static Timer subscriptionTimer = null;
    private static CosmosHelper cosmos;
    private GraphServiceClient graphClient;
    private TelemetryClient telemetryClient;
    public NotificationsController(IConfiguration config, ILogger<NotificationsController> logger)
    {
        _logger = logger;
        //logger.LogWarning("NotificationsController called!");
        this.Configuration = config;
        if (cosmos == null) {
            cosmos = new CosmosHelper(config);
        }
        
        TelemetryConfiguration telemetryConfig = TelemetryConfiguration.CreateDefault();
        telemetryConfig.InstrumentationKey = Configuration["APPINSIGHTS_INSTRUMENTATIONKEY"];
        this.telemetryClient = new TelemetryClient(telemetryConfig);
        //this.telemetryClient.InstrumentationKey = Configuration["APPINSIGHTS_INSTRUMENTATIONKEY"];
        
    }

    [HttpGet]
    public ActionResult<string> Get()
    {
        this.telemetryClient.TrackPageView("Notifications");
        if (graphClient == null) {
            graphClient = GraphHelper.Instance.GetGraphClient();
        }

        var sub = new Microsoft.Graph.Subscription();
        sub.ChangeType = "created, updated";
        //sub.NotificationUrl = Configuration["Ngrok"] + "/api/notifications";
        sub.NotificationUrl = Configuration["NotificationsEndpointUri"];
        //sub.Resource = "/users";
        sub.Resource = Configuration["SubRessource"];
        sub.ExpirationDateTime = DateTime.UtcNow.AddMinutes(Convert.ToDouble(Configuration["ExpirationDateTimeMinutes"]));
        sub.ClientState = "SecretClientState";
        //Console.WriteLine(sub.NotificationUrl);

        var newSubscription = graphClient
            .Subscriptions
            .Request()
            .AddAsync(sub).Result;

        Subscriptions[newSubscription.Id] = newSubscription;

        if(subscriptionTimer == null)
        {
            subscriptionTimer = new Timer(CheckSubscriptions, null, 5000, 15000);
        }
        var html = $"Subscribed. Id: {newSubscription.Id} Expiration: {newSubscription.ExpirationDateTime}";
        return $"{html}";
    } // Get()

    public ActionResult<string> Post([FromQuery]string validationToken = null)
    {
      // handle validation
        if(!string.IsNullOrEmpty(validationToken))
        {
            //Console.WriteLine($"Received Token: '{validationToken}'");
            //_logger.LogWarning($"Received Token: '{validationToken}'");
            this.telemetryClient.TrackEvent("Validation",new Dictionary<string, string>{{"ValidationToken",$"{validationToken}"}});
            return Ok(validationToken);
        }

        // handle notifications
        using (StreamReader reader = new StreamReader(Request.Body))
        {
            Task<string> content = reader.ReadToEndAsync();

            //Console.WriteLine(content.GetAwaiter().GetResult());

            var notifications = JsonConvert.DeserializeObject<Notifications>(content.GetAwaiter().GetResult());
            //Console.WriteLine(content.GetAwaiter().GetResult());
            
            if (this.graphClient == null) {
                this.graphClient = GraphHelper.Instance.GetGraphClient();
            }
            var accessToken = GraphHelper.Instance.GetAccessToken().Result;

            foreach(var notification in notifications.Items)
            {
                _logger.LogWarning($"Received notification: '{notification.Resource}', {notification.ResourceData?.Id}");
                this.telemetryClient.TrackEvent("Received notification", new Dictionary<string, string>{{$"{notification.Resource}",$"{notification.ResourceData?.Id}"}});
                // Calls abrufen
                string callRecordStr = notification.ResourceData?.Id.ToString();
                
                var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", accessToken);
                var con = client.GetAsync($"https://graph.microsoft.com/v1.0/communications/callrecords/{callRecordStr}").Result;
                
                var jsonString = con.Content.ReadAsStringAsync().Result;
                
                // Update CosmosDB with notifications
                try
                {
                    cosmos.AddItemToContainerAsync(jsonString).GetAwaiter().GetResult();
                }
                catch (Exception e)
                {
                   var dict = new Dictionary<string, string>{{$"message",$"{e.Message}"},{$"Stacktrace",$"{e.StackTrace}"}};
                   telemetryClient.TrackEvent("EXCEPTION THROWN", dict);
                }

            } // foreach
        }
        // use deltaquery to query for all updates
        CheckForUpdates();
        return Ok();
    } // Post([FromQuery]string validationToken = null)

    private void CheckSubscriptions(Object stateInfo)
    {
        AutoResetEvent autoEvent = (AutoResetEvent)stateInfo;
        if (stateInfo != null) {
            var dict = new Dictionary<string, string>{{$"stateInfo",$"{stateInfo.ToString()}"}};
            this.telemetryClient.TrackEvent("CheckSubscription", dict);
        }
        // _logger.LogWarning($"Checking subscriptions {DateTime.Now.ToString("h:mm:ss.fff")}");
        // _logger.LogWarning($"Current subscription count {Subscriptions.Count()}");

        foreach(var subscription in Subscriptions)
        {
            // if the subscription expires in the next 2 min, renew it
            if(subscription.Value.ExpirationDateTime < DateTime.UtcNow.AddMinutes(122))
            {
                RenewSubscription(subscription.Value);
            }
        }
    } // CheckSubscriptions(Object stateInfo)

    private void RenewSubscription(Subscription subscription)
    {
        //_logger.LogWarning($"Current subscription: {subscription.Id}, Expiration: {subscription.ExpirationDateTime}");
        //subscription.ExpirationDateTime = DateTime.UtcNow.AddMinutes(122);

        var subscription2 = new Subscription
        {
            ExpirationDateTime = DateTime.UtcNow.AddMinutes(Convert.ToDouble(Configuration["ExpirationDateTimeMinutes"]))
        };

        var foo = graphClient
            .Subscriptions[subscription.Id]
            .Request()
            .UpdateAsync(subscription2).Result;

        this.telemetryClient.TrackEvent("Renewed Subscription", new Dictionary<string, string>{
                {$"Subscription",$"{subscription.Id}"},
                {"New Expiration","{subscription2.ExpirationDateTime}"}
            });
        //_logger.LogWarning($"Renewed subscription: {subscription.Id}, New Expiration: {subscription2.ExpirationDateTime}");
    } // RenewSubscription(Subscription subscription)

    private static object DeltaLink = null;
    private static IUserDeltaCollectionPage lastPage = null;

    private void CheckForUpdates()
    {
        //var graphClient = GetGraphClient();
        
        // get a page of users
        var users = GetUsers(graphClient, DeltaLink);
        if (users == null) return;
        // OutputUsers(users);

        // go through all of the pages so that we can get the delta link on the last page.
        while (users.NextPageRequest != null)
        {
            users = users.NextPageRequest.GetAsync().Result;
            //OutputUsers(users);
        }

        object deltaLink;

        if (users.AdditionalData.TryGetValue("@odata.deltaLink", out deltaLink))
        {
            DeltaLink = deltaLink;
        }
    } // CheckForUpdates()

    private void OutputUsers(IUserDeltaCollectionPage users)
    {
        foreach(var user in users)
            {
                var message = $"User: {user.Id}, {user.GivenName} {user.Surname}";
                //_logger.LogWarning(message);
            }
    } // OutputUsers(IUserDeltaCollectionPage users)

    private IUserDeltaCollectionPage GetUsers(GraphServiceClient graphClient, object deltaLink)
    {
        IUserDeltaCollectionPage page;

        if(lastPage == null)
        {
            page = graphClient
            .Users
            .Delta()
            .Request()
            .GetAsync()
            .Result;

        }
        else
        {
            if (deltaLink == null) {
                return null;
            } else {
                lastPage.InitializeNextPageRequest(graphClient, deltaLink.ToString());
                page = lastPage.NextPageRequest.GetAsync().Result;    
            }

        }

        lastPage = page;
        return page;
    } // GetUsers(GraphServiceClient graphClient, object deltaLink)
  }
}