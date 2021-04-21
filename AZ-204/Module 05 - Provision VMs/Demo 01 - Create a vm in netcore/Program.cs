using System;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.Compute.Fluent;
using Microsoft.Azure.Management.Compute.Fluent.Models;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;

using System.Diagnostics;

namespace Demo_01___Create_a_vm_in_netcore
{
  class Program
  {
    // Static rng value
    static String rng = new Random().Next(100000, 999999).ToString();
    static void Main(string[] args)
    {
      var credentials = SdkContext.AzureCredentialsFactory.FromFile("../../Credentials/authfile.json");

      var azure = Azure
          .Configure()
          .WithLogLevel(HttpLoggingDelegatingHandler.Level.Basic)
          .Authenticate(credentials)
          .WithSubscription(credentials.DefaultSubscriptionId);

      var groupName = "az204_" + rng;
      var vmName = "az204" + rng;
      var location = "northeurope";

      Console.WriteLine("Creating Resource Group...");
      var resourceGroup = azure.ResourceGroups.Define(groupName)
          .WithRegion(location)
          .Create();

      // Optionally create availability set for HA scenarios
      /*
      IAvailabilitySet availabilitySet;
      Console.WriteLine("Creating availability set...");
      try
      {
          availabilitySet = azure.AvailabilitySets.Define("myAVSet")
              .WithRegion(location)
              .WithExistingResourceGroup(groupName)
              .WithSku(AvailabilitySetSkuTypes.Aligned)
              .Create();    
      }
      catch (System.Threading.Tasks.TaskCanceledException e)
      {
          Console.WriteLine(e.Message);
          throw;
      }
      */

      Console.WriteLine("Creating public IP address...");
      var publicIPAddress = azure.PublicIPAddresses.Define("myPublicIP")
          .WithRegion(location)
          .WithExistingResourceGroup(groupName)
          .WithDynamicIP()
          .Create();

      Console.WriteLine("Creating virtual network...");
      var network = azure.Networks.Define("myVnet")
          .WithRegion(location)
          .WithExistingResourceGroup(groupName)
          .WithAddressSpace("10.0.0.0/16")
          .WithSubnet("mySubnet", "10.0.0.0/24")
          .Create();

      Console.WriteLine("Creating network interface...");
      var networkInterface = azure.NetworkInterfaces.Define("myNIC")
          .WithRegion(location)
          .WithExistingResourceGroup(groupName)
          .WithExistingPrimaryNetwork(network)
          .WithSubnet("mySubnet")
          .WithPrimaryPrivateIPAddressDynamic()
          .WithExistingPrimaryPublicIPAddress(publicIPAddress)
          .Create();

      Console.WriteLine("Creating virtual machine...");
      azure.VirtualMachines.Define(vmName)
          .WithRegion(location)
          .WithExistingResourceGroup(groupName)
          .WithExistingPrimaryNetworkInterface(networkInterface)
          .WithLatestWindowsImage("MicrosoftWindowsServer", "WindowsServer", "2012-R2-Datacenter")
          .WithAdminUsername("azureuser")
          .WithAdminPassword("Pa$$word123!")
          //.WithExistingAvailabilitySet(availabilitySet)
          .WithSize(VirtualMachineSizeTypes.StandardDS1V2)
          .Create();

      var vm = azure.VirtualMachines.GetByResourceGroup(groupName, vmName);

      Console.WriteLine("Stopping vm...");
      vm.PowerOff();
      vm.Deallocate();

      Console.WriteLine("Resizing vm...");
      vm.Update()
          .WithSize(VirtualMachineSizeTypes.StandardB4ms)
          .Apply();

      Console.WriteLine("Adding data disk to vm...");
      vm.Update()
          .WithNewDataDisk(2, 0, CachingTypes.ReadWrite)
          .Apply();

      Console.WriteLine("Starting vm...");
      vm.Start();
    }
  }
}
