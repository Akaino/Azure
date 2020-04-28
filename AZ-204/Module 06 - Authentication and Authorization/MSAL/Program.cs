using System;
using System.Linq;
using Microsoft.Identity.Client;
namespace MSAL
{
    class Program
    {
        static void Main(string[] args)
        {
            string tenant = "karoth.onmicrosoft.com";
            string clientId = "";
            string tenantId = "";
            string authority = $"https://login.microsoftonline.com/{tenant}";
            // Create MSAL context using AAD authority
            IPublicClientApplication app;
            app = PublicClientApplicationBuilder.Create(clientId)
            .WithRedirectUri("http://localhost") // needs to be manually configured in App registration authenticatoin block!
                    .Build();
            //app = PublicClientApplicationBuilder.Create(clientId)
            //    .WithAuthority(AzureCloudInstance.AzurePublic, tenantId)
            //    .Build();
            var scopes = new string[] { "user.read" };
            var token = app.AcquireTokenInteractive(scopes)
                .ExecuteAsync().GetAwaiter().GetResult();
            Console.WriteLine(token);
        }
    }
}
