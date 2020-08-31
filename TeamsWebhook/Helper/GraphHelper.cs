
using Microsoft.Graph;

using Microsoft.Identity.Client;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;

using Microsoft.Extensions.Configuration;

namespace TeamsWebhook {
    class GraphHelper {
        private static readonly GraphHelper instance = new GraphHelper();
        private static readonly IConfiguration Configuration;

        static GraphHelper()
        {
            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("appsettings.json");
            Configuration = configurationBuilder.Build();
        }

        private GraphHelper()
        {
            
        }

        public static GraphHelper Instance
        {
            get
            {
                return instance;
            }
        }

        public GraphServiceClient GetGraphClient()
        {
            var graphClient = new GraphServiceClient(new DelegateAuthenticationProvider((requestMessage) => {

                    // get an access token for Graph
                    var accessToken = GetAccessToken().Result;
                    requestMessage
                        .Headers
                        .Authorization = new AuthenticationHeaderValue("bearer", accessToken);

                    return Task.FromResult(0);
                })
            );

            return graphClient;
        } // GetGraphClient()

        public async Task<string> GetAccessToken()
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
        } // GetAccessToken()
    } // class
} // namespace