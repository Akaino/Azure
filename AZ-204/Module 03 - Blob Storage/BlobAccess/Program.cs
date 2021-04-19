using System;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
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

            CloudStorageAccount account = CloudStorageAccount.Parse(connStr);

            CloudBlobClient client = account.CreateCloudBlobClient();

            var date = DateTime.Now;

            CloudBlobContainer container = client.
                GetContainerReference(date.Millisecond + "myblobcontainer");
            
            var exists = await container.CreateIfNotExistsAsync();

            CloudBlockBlob blob = container.
                GetBlockBlobReference(date.Millisecond + "myblob.png");

            await blob.UploadFromFileAsync("fileToUpload.png");
            return 0;
        }
    }
}
