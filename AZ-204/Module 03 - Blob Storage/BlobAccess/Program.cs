using System;
using Azure.Storage;
using Azure.Storage.Blobs;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Threading.Tasks;

namespace BlobAccess
{
  class Program
  {
    static JObject azureConfig = JObject.Parse(File.ReadAllText("azureConfig.json"));

    // Batch Account credentials
    static String storageConnectionString = azureConfig.GetValue("StorageConnectionString").ToString();

    static async Task<int> Main(string[] args)
    {
      Console.WriteLine("Accessing Blob Storage");
      string connStr = storageConnectionString;
      var date = DateTime.Now;
      
      try
      {
        BlobContainerClient containerClient = new BlobContainerClient(connStr, date.Millisecond + "myblobcontainer");
        await containerClient.CreateIfNotExistsAsync();
        
        Console.WriteLine("Created Container");

        try
        {
          BlobClient blobClient = containerClient.GetBlobClient(date.Millisecond + "myblob.png");
          await blobClient.UploadAsync("fileToUpload.png");
          
          Console.WriteLine("Upload Completed");
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

      return 0;
    }
  }
}
