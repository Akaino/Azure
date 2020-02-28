using System;

using System.Threading.Tasks;
using Microsoft.Azure.Services.AppAuthentication;

using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Rest;

namespace Managed_Identity
{
    class Program
    {
        static async Task Main(string[] args)
        {
            /**
            Für die lokale Entwicklung ruft AzureServiceTokenProvider Token mithilfe von Visual Studio, 
            der Azure-Befehlszeilenschnittstelle (CLI) oder der integrierten Azure AD-Authentifizierung ab. 
            Jede Option wird nacheinander ausprobiert, und die Bibliothek verwendet die erste Option, 
            die erfolgreich ausgeführt wird. Wenn keine Option funktioniert, wird eine AzureServiceTokenProviderException-Ausnahme 
            mit detaillierten Informationen ausgelöst.
            */

            AzureServiceTokenProvider azureServiceTokenProvider = new AzureServiceTokenProvider();
            
            string tenantId = "...";
            AzureServiceTokenProvider tokenProvider = new AzureServiceTokenProvider();
            IResourceManager resourceManager = ResourceManager
                .Authenticate(new AzureCredentials(
                    new TokenCredentials(tokenProvider.GetAccessTokenAsync("https://management.azure.com/").Result),
                    new TokenCredentials(tokenProvider.GetAccessTokenAsync("https://graph.windows.net/").Result),
                    tenantId,
                    AzureEnvironment.AzureGlobalCloud))
                .WithDefaultScubscription();

            
        }
    }
}
