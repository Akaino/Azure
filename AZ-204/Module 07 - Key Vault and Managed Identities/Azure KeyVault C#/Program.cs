using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Newtonsoft.Json.Linq;
using System.IO;

namespace Azure_KeyVault_C_
{
    class Program
    {
        static JObject azureConfig = JObject.Parse(File.ReadAllText("azureConfig.json"));

        // Batch Account credentials
        static String KeyVaultURL = azureConfig.GetValue("KeyVaultURL").ToString();
        static String AADAppRegistrationID = azureConfig.GetValue("AADAppRegistrationID").ToString();
        static String AADAppRegistrationSecret = azureConfig.GetValue("AADAppRegistrationSecret").ToString();
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder();
            builder.AddAzureKeyVault(KeyVaultURL, AADAppRegistrationID, AADAppRegistrationSecret);
            var config = builder.Build();

            Console.WriteLine(config["supersecret"]); // not case-sensitive

        }
    }
}
