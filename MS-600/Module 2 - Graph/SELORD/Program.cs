using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Identity.Client;
using Microsoft.Graph;

using System.Net.Http;
using System.Net.Http.Headers;


namespace SELORD
{
    class Program
    {
        
        private static string clientID = "e433aa6a-8116-4c18-82bf-031768fe9ef9";
        private static string tenant = "5d8b74a9-cfa0-4f65-9d4f-56b92787a8c3";
        private static string clientSecret = "0DcY.L_0q.Js0mVq4~eO3ocko.ZcVnqhh5";
        private static string instance = $"https://login.microsoftonline.com/{tenant}/v2.0";
        
        //Permissions: Mail.Read, User.Read
        private static List<string> scopes = new List<string>();

        static void Main(string[] args)
        {
            scopes.Add("https://graph.microsoft.com/.default");

            var ccs = ConfidentialClientApplicationBuilder.Create(clientID)
                                                          .WithAuthority(instance)
                                                          .WithClientSecret(clientSecret)
                                                          .Build();
            
            GraphServiceClient graphServiceClient = new GraphServiceClient(new DelegateAuthenticationProvider(async (requestMessage) => {
                    var authResult = await ccs
                        .AcquireTokenForClient(scopes)
                        .ExecuteAsync();

                    requestMessage.Headers.Authorization = 
                        new AuthenticationHeaderValue("Bearer", authResult.AccessToken);
                })
            );
            
            var httpClient = new HttpClient();
            var graphClient = new GraphServiceClient(httpClient);
            
            //var users = graphServiceClient.Users.Request().GetAsync().GetAwaiter().GetResult();
            var users = graphServiceClient.Users.Request().Filter("startswith(displayName, 'T')").GetAsync().GetAwaiter().GetResult(); 
            //var users = graphServiceClient.Users.Request().Select(u => new {u.DisplayName, u.City}).GetAsync().GetAwaiter().GetResult();
            //var users = graphServiceClient.Users["admin@devtobi.onmicrosoft.com"].Messages.Request().GetAsync().GetAwaiter().GetResult();
            
            foreach(var user in users)
            { 
                Console.WriteLine(user.DisplayName);
                //Console.WriteLine(user.ReceivedDateTime + " - " + user.Subject);
            }
        }
    }
}

