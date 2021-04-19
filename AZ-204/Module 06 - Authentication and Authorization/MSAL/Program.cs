using System;
using System.Linq;
using Microsoft.Identity.Client;
namespace MSAL
{
  class Program
  {
    static void Main(string[] args)
    {
      string tenant = "1ec6f2a5-9c5b-4e9a-94fe-6e8080621457";
      string clientId = "d78bc4a4-8321-447d-9df5-6dffca91b58c";
      //string tenantId = "2df5d932-1f85-495e-a240-79c142d63c4a";
      string authority = $"https://login.microsoftonline.com/{tenant}";
      // Create MSAL context using AAD authority
      IPublicClientApplication app;
      app = PublicClientApplicationBuilder.Create(clientId)
      .WithAuthority(authority)
      .WithRedirectUri("http://localhost") // needs to be manually configured in App registration authenticatoin block!
      .Build();
      //app = PublicClientApplicationBuilder.Create(clientId)
      //    .WithAuthority(AzureCloudInstance.AzurePublic, tenantId)
      //    .Build();
      var scopes = new string[] { "user.read" };
      AuthenticationResult token = null;
      var accounts = app.GetAccountsAsync().GetAwaiter().GetResult();
   
      try
      {
        token = app.AcquireTokenSilent(scopes, accounts.FirstOrDefault())
        .ExecuteAsync().GetAwaiter().GetResult();
      }
      catch (MsalUiRequiredException ex)
      {
        Console.WriteLine($"MsalUiRequiredException: {ex.Message}");

        token = app.AcquireTokenInteractive(scopes)
        .ExecuteAsync().GetAwaiter().GetResult();
      }

      Console.WriteLine(token.AccessToken);
    }
  }
}
