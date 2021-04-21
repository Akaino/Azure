using Microsoft.Graph;
using Microsoft.Identity.Client;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace GRPCYC
{
  public class Auth : IAuthenticationProvider
  {
    private IConfidentialClientApplication app;

    public Auth(string appId, string authority, string secret)
    {
      app = ConfidentialClientApplicationBuilder.Create(appId)
     .WithClientSecret(secret)
     .WithAuthority(new Uri(authority))
     .Build();
    }

    public async Task<string> GetAccessToken()
    {
      string[] scopes = new string[] { "https://graph.microsoft.com/.default" };

      AuthenticationResult result = null;
      try
      {
        result = await app.AcquireTokenForClient(scopes)
                         .ExecuteAsync();
      }
      catch (MsalUiRequiredException ex)
      {
        // The application doesn't have sufficient permissions.
        // - Did you declare enough app permissions during app creation?
        // - Did the tenant admin grant permissions to the application?
        Console.WriteLine(ex.Message);
      }
      catch (MsalServiceException ex) when (ex.Message.Contains("AADSTS70011"))
      {
        // Invalid scope. The scope has to be in the form "https://resourceurl/.default"
        // Mitigation: Change the scope to be as expected.
      }

      return result.AccessToken;
    }

    // This is the required function to implement IAuthenticationProvider
    // The Graph SDK will call this function each time it makes a Graph
    // call.
    public async Task AuthenticateRequestAsync(HttpRequestMessage requestMessage)
    {
      requestMessage.Headers.Authorization =
          new AuthenticationHeaderValue("bearer", await GetAccessToken());
    }
  }
}