using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Identity.Client;
using Microsoft.Graph;

using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;

namespace SELORD
{
  class Program
  {
    static JObject azureConfig = JObject.Parse(System.IO.File.ReadAllText("appSettings.json"));
    static String clientID = azureConfig.GetValue("CLIENT_ID").ToString();
    static String authority = azureConfig.GetValue("AUTHORITY").ToString();
    static String secret = azureConfig.GetValue("SECRET").ToString();

    //Permissions: Mail.Read, User.Read
    private static List<string> scopes = new List<string>();

    static void Main(string[] args)
    {
      scopes.Add("https://graph.microsoft.com/.default");

      var ccs = ConfidentialClientApplicationBuilder.Create(clientID)
                                                    .WithAuthority(authority)
                                                    .WithClientSecret(secret)
                                                    .Build();

      GraphServiceClient graphServiceClient = new GraphServiceClient(new DelegateAuthenticationProvider(async (requestMessage) =>
      {
        var authResult = await ccs.AcquireTokenForClient(scopes).ExecuteAsync();

        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authResult.AccessToken);
      }));

      var httpClient = new HttpClient();
      var graphClient = new GraphServiceClient(httpClient);

      //var users = graphServiceClient.Users.Request().GetAsync().GetAwaiter().GetResult();
      var users = graphServiceClient.Users.Request().Filter("startswith(displayName, 'T')").GetAsync().GetAwaiter().GetResult();
      //var users = graphServiceClient.Users.Request().Select(u => new {u.DisplayName, u.City}).GetAsync().GetAwaiter().GetResult();
      //var users = graphServiceClient.Users["admin@devtobi.onmicrosoft.com"].Messages.Request().GetAsync().GetAwaiter().GetResult();

      foreach (var user in users)
      {
        Console.WriteLine(user.DisplayName);
        //Console.WriteLine(user.ReceivedDateTime + " - " + user.Subject);
      }
    }
  }
}

