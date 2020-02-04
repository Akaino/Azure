using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.Azure.Management.ContainerService.Fluent;
using Microsoft.Azure.Management.ContainerService.Fluent.Models;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Azure.Management.Fluent;

using Newtonsoft.Json.Linq;

namespace Demo_05___AKS
{
    class Program
    {

        // Create an SSH Key from your machine and add it to azureConfig.json
        // ssh-keygen -t rsa -b 2048 -v -f certName
        // Need to escape special characters!! (/ == //)


        static JObject azureConfig = JObject.Parse(File.ReadAllText("azureConfig.json"));

        static void Main(string[] args)
        {
            AsyncMain(args).GetAwaiter().GetResult();
        }

        static async Task AsyncMain(string[] args) {
            var credentials = SdkContext.AzureCredentialsFactory.FromFile("../../Credentials/authfile.json");
            var azure = Azure
                .Configure()
                .WithLogLevel(HttpLoggingDelegatingHandler.Level.Basic)
                .Authenticate(credentials)
                .WithDefaultSubscription();

            Console.WriteLine("Your subscription ID: \r\n" + azure.SubscriptionId);
            Console.WriteLine("Creating an AKS Cluster!");

            String rootUser = azureConfig.GetValue("RootUser").ToString();
            String rgName = azureConfig.GetValue("ResourceGroup").ToString();
            String aksName = azureConfig.GetValue("ClusterName").ToString();
            String location = azureConfig.GetValue("Location").ToString();
            String sshPublicKey = azureConfig.GetValue("SshPublicKey").ToString();
            String clientSecret = JObject.Parse(File.ReadAllText("../../Credentials/authfile.json")).GetValue("clientSecret").ToString();
            try {
                Console.WriteLine("Trying to create the cluster...");
                IKubernetesCluster cluster = await azure.KubernetesClusters.Define(aksName)
                    .WithRegion(location)
                    .WithNewResourceGroup(rgName)
                    .WithLatestVersion()
                    .WithRootUsername(rootUser)
                    .WithSshKey(sshPublicKey)
                    .WithServicePrincipalClientId(credentials.ClientId)
                    .WithServicePrincipalSecret(clientSecret)
                    .DefineAgentPool("agentpool")
                        .WithVirtualMachineSize(ContainerServiceVMSizeTypes.StandardA2)
                        .WithAgentPoolVirtualMachineCount(2)
                        .Attach()
                    .WithDnsPrefix("dns-"+aksName)
                    .CreateAsync();

                Console.WriteLine("Created Kubernetes Cluster: " + cluster.Id);
                Console.WriteLine(cluster);

                Console.WriteLine("Updating Kubernetes Cluster. More VMs!!: " + cluster.Id);
                await cluster.Update()
                    .WithAgentPoolVirtualMachineCount(4)
                    .ApplyAsync();

                Console.WriteLine("Updated Kubernetes Cluster: " + cluster.Id);
                Console.WriteLine(cluster);
            }  catch (Exception g)
                {
                    Console.WriteLine(g);
                }
            finally {
                try
                {
                    Console.WriteLine("Deleting Resource Group: " + rgName);
                    await azure.ResourceGroups.BeginDeleteByNameAsync(rgName);
                    Console.WriteLine("Deleted Resource Group: " + rgName);
                }
                catch (NullReferenceException)
                {
                    Console.WriteLine("Did not create any resources in Azure. No clean up is necessary");
                }
                catch (Exception g)
                {
                    Console.WriteLine(g);
                }
            }
        }
    }
}
