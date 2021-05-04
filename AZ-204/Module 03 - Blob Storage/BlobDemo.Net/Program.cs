using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Newtonsoft.Json.Linq;

namespace BlobDemo.Net
{
  class Program
  {
    static JObject azureConfig = JObject.Parse(File.ReadAllText("azureConfig.json"));

    // Storage Account credentials
    static String storageConnectionString = azureConfig.GetValue("StorageConnectionString").ToString();

    static void Main(string[] args)
    {
      Console.WriteLine("Azure Blob Storage Demo\n");
      // Run the examples asynchronously, wait for the results before proceeding
      ProcessAsync().GetAwaiter().GetResult();
    }

    private static async Task ProcessAsync()
    {
      try{
        Console.WriteLine("Press enter to create Container and File.");Console.ReadLine();
        
        string blob = await CreateContainer(storageConnectionString);
        
        Console.WriteLine("Press enter to DownloadFile.");Console.ReadLine();
        
        await DownloadBlob(storageConnectionString, blob);
        
        Console.WriteLine("Press enter to read Infos.");Console.ReadLine();
        
        await ReadInfos(storageConnectionString);
        
        Console.WriteLine("Press enter to delete container.");Console.ReadLine();
      }
      finally{
        await DeleteContainer(storageConnectionString);

        Console.WriteLine("Press enter to exit the sample application.");Console.ReadLine();
      }
    }

    private static async Task ReadInfos(string connString)
    {
      BlobServiceClient storageAccount;

      try
      {
        storageAccount = new BlobServiceClient(connString);
        AccountInfo accountInfo = await storageAccount.GetAccountInfoAsync();

        Console.WriteLine("AccountKind: " + accountInfo.AccountKind);
        Console.WriteLine("AccountSku: " + accountInfo.SkuName);

        if (accountInfo.SkuName == SkuName.StandardRagrs)
        {
          BlobServiceStatistics stats = await storageAccount.GetStatisticsAsync();
          Console.WriteLine("Statistics (GeoReplication): " + stats.GeoReplication);
        }

        var containers = storageAccount.GetBlobContainers();

        foreach (var container in containers)
        {
          Console.WriteLine("checking Container: " + container.Name);

          if (container.IsDeleted ?? true)
          {
            BlobContainerClient containerClient = new BlobContainerClient(connString, container.Name);
            BlobContainerProperties containerProperties = await containerClient.GetPropertiesAsync();

            Console.WriteLine("Container has LegalHold? " + containerProperties.HasLegalHold);
            Console.WriteLine("Container is Immutable? " + containerProperties.HasImmutabilityPolicy);

            await foreach (BlobItem blob in containerClient.GetBlobsAsync())
            {
              Console.WriteLine("Blob: " + blob.Name);
              Console.WriteLine("     Type Of:" + blob.Properties.BlobType);
              Console.WriteLine(blob.Properties.ContentLength > 1024 ?
                                       "     Size (KB):" + (blob.Properties.ContentLength / 1024) :
                                       "     Size (byte):" + blob.Properties.ContentLength);
            }

          }
          else
          {
            Console.WriteLine("    >>>> Is Deleted!" + container.Properties.RemainingRetentionDays);
          }
        }
      }
      catch (Exception e)
      {
        Console.WriteLine("OH SNAP!: " + e.Message);
      }
    }
   
    private static async Task<string> CreateContainer(string connString)
    {
      string _fileName = "";

      try
      {
        BlobContainerClient containerClient = new BlobContainerClient(connString, "my-container");
        await containerClient.CreateIfNotExistsAsync();

        try
        {
          var (fileName, filePath) = CreateSampleFile();
          _fileName = fileName;

          BlobClient blobClient = containerClient.GetBlobClient(fileName);
          await blobClient.UploadAsync(filePath);
        }
        catch (Exception e)
        {
          Console.WriteLine("Blob Exception: " + e.Message);
        }
      }
      catch (Exception e)
      {
        Console.WriteLine("OH SNAP!: " + e.Message);
      }

      return _fileName;
    }

    private static async Task DownloadBlob(string connString, string blobName)
    {
      // Download the blob to a local file, using the reference created earlier.
      // Append the string "_DOWNLOADED" before the .txt extension so that you
      // can see both files in MyDocuments.
      string destinationFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "BlobDemo_DOWNLOADED.txt");
      Console.WriteLine("Downloading blob to {0}", destinationFile);

      try
      {
        BlobContainerClient containerClient = new BlobContainerClient(connString, "my-container");
        await containerClient.CreateIfNotExistsAsync();

        try
        {
          BlobClient blobClient = containerClient.GetBlobClient(blobName);
          await blobClient.DownloadToAsync(destinationFile);
        }
        catch (Exception e)
        {
          Console.WriteLine("Blob Exception: " + e.Message);
        }
      }
      catch (Exception e)
      {
        Console.WriteLine("OH SNAP!: " + e.Message);
      }
    }

    private static async Task DeleteContainer(string connString)
    {
      try
      {
        BlobContainerClient containerClient = new BlobContainerClient(connString, "my-container");
        await containerClient.DeleteIfExistsAsync();
      }
      catch (Exception e)
      {
        Console.WriteLine("OH SNAP!: " + e.Message);
      }
    }
    
    private static (string, string) CreateSampleFile()
    {
      string localPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
      string localFileName = "BlobDemo_" + Guid.NewGuid().ToString() + ".txt";
      string sourceFile = Path.Combine(localPath, localFileName);
      // Write text to the file.
      File.WriteAllText(sourceFile, "Hello, World!");
      Console.WriteLine("\r\nTemp file = {0}", sourceFile);

      return (localFileName, sourceFile);
    }
  
    //TODO: Generate SAS Policy 
  }
}
