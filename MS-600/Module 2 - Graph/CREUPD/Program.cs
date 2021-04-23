using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Identity.Client;
using Microsoft.Graph;

using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;

namespace CREUPD
{
  class Program
  {
    static JObject azureConfig = JObject.Parse(System.IO.File.ReadAllText("appsettings.json"));
    static String clientID = azureConfig.GetValue("CLIENT_ID").ToString();
    static String authority = azureConfig.GetValue("AUTHORITY").ToString();
    static String secret = azureConfig.GetValue("SECRET").ToString();

    //Permissions: User.Read, User.ReadWrite.All
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

      //Wird später zum neuen Manager
      var manager = graphServiceClient.Users["mctest@devtobi.onmicrosoft.com"].Request().GetAsync().GetAwaiter().GetResult();

      var newUser = new User
      {
        DisplayName = "Test User",
        UserPrincipalName = "testu@devtobi.onmicrosoft.com",
        MailNickname = "testu",
        AccountEnabled = true,
        PasswordProfile = new PasswordProfile
        {
          Password = "ChangeMe1!",
          ForceChangePasswordNextSignIn = false
        },
        GivenName = "Test",
        Department = "TestGuys",
        Surname = "User"
      };

      //User anlegen
      try
      {
        var userCall = graphServiceClient.Users.Request().AddAsync(newUser).GetAwaiter().GetResult();
      }
      catch (Exception e)
      {
        Console.WriteLine($"ERROR: {e.Message}");
      }
      finally
      {
        var timedKillCommand = new Timer((e) =>
        {
          Console.WriteLine("Killing The User");
          var result = graphServiceClient.Users[newUser.UserPrincipalName].Request().DeleteAsync();

          Console.WriteLine("Assimilated");

          //kill
          Environment.Exit(0);
        }, null, ((1000 * 60) * 1), Timeout.Infinite);
      }

      Console.WriteLine("User created? I\'m checking");
      var checkUser = graphServiceClient.Users.Request().Filter($"displayName eq '{newUser.DisplayName}'").GetAsync().GetAwaiter().GetResult();

      if (checkUser != null && checkUser.Count > 0)
      {
        var checkUserContent = checkUser.CurrentPage[0];
        
        Console.WriteLine("User created! Let\'s assign a manager");
        graphServiceClient.Users[checkUserContent.Id].Manager.Reference.Request().PutAsync(manager.Id).GetAwaiter();

        Console.WriteLine("Manager Assigned! Let\'s rename him ....");
        var userUpdate = new User
        {
          DisplayName = "Updated thisGuy",
          OfficeLocation = "Knowhere"
        };

        var updatedUser = graphServiceClient.Users[checkUserContent.Id].Request().UpdateAsync(userUpdate).GetAwaiter();
        checkUserContent = graphServiceClient.Users[checkUserContent.Id].Request().Select(u => new { u.DisplayName, u.Department, u.Id }).GetAsync().GetAwaiter().GetResult();
       
        Console.WriteLine($"{checkUserContent.DisplayName} working at {checkUserContent.Department}");
      }
      else
      {
        Console.WriteLine("User NOT created!");
      }

      Console.ReadLine();
    }
  }
}

