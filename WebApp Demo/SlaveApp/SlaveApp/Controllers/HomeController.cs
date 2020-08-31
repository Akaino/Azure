using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using SlaveApp.Models;
using Microsoft.ApplicationInsights;

// Storage
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.Reflection.Metadata;
using Azure;

using Microsoft.Graph;
using Microsoft.Identity.Client;


using System.Net.Http;
using System.Net.Http.Headers;

namespace SlaveApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private TelemetryClient telemetry;
        private IConfiguration _configuration;
        private IPublicClientApplication _msalClient;
        private String[] _scopes;
        private IAccount _userAccount;
        public HomeController(ILogger<HomeController> logger, TelemetryClient telemetry, IConfiguration configuration)
        {
            _logger = logger;
            logger.LogWarning("Test warning for ILogger. Warnings and higher logging will automatically be captured by Application Insights.");
            logger.LogWarning("Also, dependency collection is enabled by default. (http/https, wcf calls, sqlclient information, azure storage connections, eventhub sdk, cosmos db, service hub...)");
            logger.LogWarning("For .Net Core Applications Microsoft.ApplicationInsights.DependencyCollector is required.");
            logger.LogWarning("Also, to catch other dependencies, there is a TrackDependency call.");

            this.telemetry = telemetry;
            this._configuration = configuration;


    }

        public IActionResult Index()
        {
            this.telemetry.TrackEvent("Called index!");
            return View();
        }

        public IActionResult Privacy()
        {
            this.telemetry.TrackEvent("Called privacy!");
            this.telemetry.TrackMetric("MyMetric", 0.815);
            return View();
        }

        public IActionResult Images()
        {
            var storageConnectionString = _configuration["StorageConnectionString"];
            BlobServiceClient blobClient = new BlobServiceClient(storageConnectionString);
            var containers = EnumerateContainersAsync(blobClient).GetAwaiter().GetResult();
            var blobitems = EnumerateBlobsAsync(blobClient, "media").GetAwaiter().GetResult();
            List<String> l = new List<string>();
            foreach (BlobItem item in blobitems)
            {
                l.Add(blobClient.Uri + "" + containers + "/" + item.Name);
                this.telemetry.TrackEvent("Found blob with name " + item.Name);
            }
            this.telemetry.TrackEvent("Called images!");
            ViewBag.MyList = l;
            return View();
        }

        

        public IActionResult Graph()
        {

            var clientID = _configuration["ClientID"]; 
            this.telemetry.TrackEvent("Called graph!");
            this._scopes = new String[] { "user.read", "user.read.all"};
            
            _msalClient = PublicClientApplicationBuilder
                .Create(clientID)
                .WithAuthority(AadAuthorityAudience.AzureAdAndPersonalMicrosoftAccount, true)
                .Build();

            var authProvider = new DeviceCodeAuthProvider(clientID, _scopes, telemetry);
            var accessToken = authProvider.GetAccessToken().Result;
            this.telemetry.TrackEvent(accessToken);
            GraphHelper.Initialize(authProvider);
            var user = GraphHelper.GetMeAsync().Result;
            this.telemetry.TrackEvent(user.DisplayName);
            var userProperties = GraphHelper.GetMyPropertiesAsync().Result;
            this.telemetry.TrackEvent(userProperties.AdditionalData.ToString());
            
            ViewBag.User = user;
            ViewBag.UserProperties = userProperties;
            return View();
        }

        

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // Storage
        private static async Task<String> EnumerateContainersAsync(BlobServiceClient client)
        {
            var containers = "";
            await foreach (BlobContainerItem container in client.GetBlobContainersAsync())
            {
                containers += container.Name;
            }
            return containers;
        }

        private static async Task<List<BlobItem>> EnumerateBlobsAsync(BlobServiceClient client, string containerName)
        {
            BlobContainerClient container = client.GetBlobContainerClient(containerName);
            List<BlobItem> blobs = new List<BlobItem>();
            await foreach (BlobItem blob in container.GetBlobsAsync())
            {
                blobs.Add(blob);
            }
            return blobs;
        }

    }
}
