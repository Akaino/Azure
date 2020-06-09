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

using System.Net.Http.Formatting;
using Microsoft.Graph;

using Microsoft.Identity.Client;
using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Microsoft.Azure.Cosmos;

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
    private CosmosClient cosmosClient;
    private Database database;
    private Container container;
    // public NotificationsController(MyConfig config)
    // {
    //   this.config = config;
    // }
    public NotificationsController(IConfiguration config, ILogger<NotificationsController> logger)
    {
        _logger = logger;
        //logger.LogWarning("NotificationsController called!");
        this.Configuration = config;
    }

    [HttpGet]
    public ActionResult<string> Get()
    {
      var graphServiceClient = GetGraphClient();

        var sub = new Microsoft.Graph.Subscription();
        sub.ChangeType = "created, updated";
        //sub.NotificationUrl = Configuration["Ngrok"] + "/api/notifications";
        sub.NotificationUrl = Configuration["NotificationsEndpointUri"];
        //sub.Resource = "/users";
        sub.Resource = Configuration["SubRessource"];
        sub.ExpirationDateTime = DateTime.UtcNow.AddMinutes(Convert.ToDouble(Configuration["ExpirationDateTimeMinutes"]));
        sub.ClientState = "SecretClientState";
        //Console.WriteLine(sub.NotificationUrl);

        var newSubscription = graphServiceClient
        .Subscriptions
        .Request()
        .AddAsync(sub).Result;

        Subscriptions[newSubscription.Id] = newSubscription;

        if(subscriptionTimer == null)
        {
            subscriptionTimer = new Timer(CheckSubscriptions, null, 5000, 15000);
        }

        return $"Subscribed. Id: {newSubscription.Id}, Expiration: {newSubscription.ExpirationDateTime}";
    }

    public ActionResult<string> Post([FromQuery]string validationToken = null)
    {
      // handle validation
        if(!string.IsNullOrEmpty(validationToken))
        {
            //Console.WriteLine($"Received Token: '{validationToken}'");
            //_logger.LogWarning($"Received Token: '{validationToken}'");
            
            return Ok(validationToken);
        }

        // handle notifications
        using (StreamReader reader = new StreamReader(Request.Body))
        {
            Task<string> content = reader.ReadToEndAsync();

            //Console.WriteLine(content.GetAwaiter().GetResult());

            var notifications = JsonConvert.DeserializeObject<Notifications>(content.GetAwaiter().GetResult());
            //Console.WriteLine(content.GetAwaiter().GetResult());
            
            var graphClient = GetGraphClient();
            var accessToken = GetAccessToken().Result;

            if (notifications.Items.Count() > 0) {
                var options = new CosmosClientOptions() { ConnectionMode = ConnectionMode.Gateway };
                this.cosmosClient = new CosmosClient(this.Configuration["cosmosUri"], this.Configuration["cosmosKey"], options);
                this.database = cosmosClient.GetDatabase(Configuration["cosmosDatabase"]);
                this.container = cosmosClient.GetContainer(database.Id, Configuration["cosmosContainer"]);
            }


            foreach(var notification in notifications.Items)
            {
                _logger.LogWarning($"Received notification: '{notification.Resource}', {notification.ResourceData?.Id}");
                
                // Calls abrufen
                string callRecordStr = notification.ResourceData?.Id.ToString();
                
                var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", accessToken);
                var con = client.GetAsync($"https://graph.microsoft.com/v1.0/communications/callrecords/{callRecordStr}").Result;
                
                var jsonString = con.Content.ReadAsStringAsync().Result;
                var json = JObject.Parse(jsonString);
                
                // cosmos
                AddItemToContainerAsync(jsonString).GetAwaiter().GetResult();

                string organizer;
                if (json != null) {
                    //Console.WriteLine($"##### ERROR CallRecord ID: {json["id"]}");

                    if (json["organizer"]["user"].Type == JTokenType.Null) {
                        organizer = json["organizer"]["phone"]["id"].ToString();
                    } else {
                        if (json["organizer"]["user"]["displayName"].Type == JTokenType.Null) {
                            organizer = json["organizer"]["user"]["id"].ToString();
                        } else {
                            organizer = organizer = json["organizer"]["user"]["displayName"].ToString();
                        }
                    }

                    //var organizer = json["organizer"]["user"]["displayName"] == null ? json["organizer"]["user"]["id"] : json["organizer"]["user"]["displayName"];
                    //_logger.LogWarning($"CallRecord ID: {json["id"]}, Start: {json["startDateTime"]}, End: {json["endDateTime"]}, Initiator: {organizer}");

                }


            }
        }

        // use deltaquery to query for all updates
        CheckForUpdates();

        return Ok();
    }

    private GraphServiceClient GetGraphClient()
    {
      var graphClient = new GraphServiceClient(new DelegateAuthenticationProvider((requestMessage) => {

          // get an access token for Graph
          var accessToken = GetAccessToken().Result;

          requestMessage
              .Headers
              .Authorization = new AuthenticationHeaderValue("bearer", accessToken);

          return Task.FromResult(0);
      }));

      return graphClient;
    }

    private async Task<string> GetAccessToken()
    {
//      IConfidentialClientApplication app = ConfidentialClientApplicationBuilder.Create(config.AppId)
        IConfidentialClientApplication app = ConfidentialClientApplicationBuilder.Create(Configuration["AppId"])
//        .WithClientSecret(config.AppSecret)
        .WithClientSecret(Configuration["AppSecret"])
        //.WithAuthority($"https://login.microsoftonline.com/{config.TenantId}")
        .WithAuthority($"https://login.microsoftonline.com/{Configuration["TenantId"]}")
        .WithRedirectUri("https://daemon")
        .Build();

      string[] scopes = new string[] { "https://graph.microsoft.com/.default" };

      var result = await app.AcquireTokenForClient(scopes).ExecuteAsync();
      return result.AccessToken;
    }

    private void CheckSubscriptions(Object stateInfo)
    {
        AutoResetEvent autoEvent = (AutoResetEvent)stateInfo;

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
    }

    private void RenewSubscription(Subscription subscription)
    {
        //_logger.LogWarning($"Current subscription: {subscription.Id}, Expiration: {subscription.ExpirationDateTime}");

        var graphServiceClient = GetGraphClient();

        //subscription.ExpirationDateTime = DateTime.UtcNow.AddMinutes(122);

        var subscription2 = new Subscription
        {
            ExpirationDateTime = DateTime.UtcNow.AddMinutes(Convert.ToDouble(Configuration["ExpirationDateTimeMinutes"]))
        };

        var foo = graphServiceClient
            .Subscriptions[subscription.Id]
            .Request()
            .UpdateAsync(subscription2).Result;

        _logger.LogWarning($"Renewed subscription: {subscription.Id}, New Expiration: {subscription2.ExpirationDateTime}");
    }

    private static object DeltaLink = null;

    private static IUserDeltaCollectionPage lastPage = null;

    private void CheckForUpdates()
    {
        var graphClient = GetGraphClient();

        // get a page of users
        var users = GetUsers(graphClient, DeltaLink);

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
    }

    private void OutputUsers(IUserDeltaCollectionPage users)
    {
        foreach(var user in users)
            {
                var message = $"User: {user.Id}, {user.GivenName} {user.Surname}";
                //_logger.LogWarning(message);
            }
    }

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
    }

    // Cosmos
    private async Task AddItemToContainerAsync(string httpResult)
        {
            var CallRecordModel = JsonConvert.DeserializeObject<CallRecordModel>(httpResult);
            var callRecordModelResponse = await this.container.ReadItemStreamAsync(CallRecordModel.Id, new PartitionKey(CallRecordModel.Id));

            if (callRecordModelResponse.StatusCode == HttpStatusCode.NotFound)
            {
                // Create an item in the container representing the Andersen family. Note we provide the value of the partition key for this item, which is "Andersen"
                try
                {
                    var item = await this.container.CreateItemAsync(CallRecordModel, new PartitionKey(CallRecordModel.Id));
                    Console.WriteLine("Created item in database with id: {0} Operation consumed {1} RUs.\n", item.Resource.Id, item.RequestCharge);
                
                }
                catch (System.Exception)
                {
                    Console.WriteLine("Item already exists. Probably...");
                    ItemResponse<CallRecordModel> item = await this.container.ReplaceItemAsync<CallRecordModel>(CallRecordModel, CallRecordModel.Id);
                }
                // Note that after creating the item, we can access the body of the item with the Resource property off the ItemResponse. We can also access the RequestCharge property to see the amount of RUs consumed on this request.
                
            }
            else
            {
                ItemResponse<CallRecordModel> item = await this.container.ReplaceItemAsync<CallRecordModel>(CallRecordModel, CallRecordModel.Id);
                //ItemResponse<CallRecordModel> item = await this.container.ReadItemAsync<CallRecordModel>(CallRecordModel.Id, new PartitionKey(CallRecordModel.Id));
                Console.WriteLine("Item in database with id: {0} already exists\n", item.Resource.Id, ". Upserted item.");
            }
        }

  }
}