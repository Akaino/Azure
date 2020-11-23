using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;

namespace BlobDemo.Net
{
  class Program
  {
    static void Main(string[] args)
    {
      Console.WriteLine("Azure Blob Storage Demo\n");
      // Run the examples asynchronously, wait for the results before proceeding
      ProcessAsync().GetAwaiter().GetResult();
      Console.WriteLine("Press enter to exit the sample application.");
      Console.ReadLine();    
    }
    private static async Task ProcessAsync()
    {
    // Copy the connection string from the portal in the variable below.
      string storageConnectionString = "DefaultEndpointsProtocol=https;AccountName=demoblobstor;AccountKey=EoDUPBjFjppLflvnaUEeaZ88ItRnQ8OKUDT9d3GcySGwd1Ay0cpLjHU7kyaUB1i5z1+329pznpy+DBItb4a2/g==;EndpointSuffix=core.windows.net";
      // Check whether the connection string can be parsed. 
      CloudStorageAccount storageAccount;
      if (CloudStorageAccount.TryParse(storageConnectionString, out storageAccount))
      {
        // Run the example code if the connection string is valid. 
        Console.WriteLine("Valid connection string.\r\n");
        // EXAMPLE CODE STARTS BELOW HERE
        //Uri bloburi = new Uri("https://demoblobstor.blob.core.windows.net/");
        CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();
        // Create a container called 'quickstartblobs' and
        // append a GUID value to it to make the name unique.
        CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("demoblobs" + Guid.NewGuid().ToString());
        await cloudBlobContainer.CreateAsync();
        Console.WriteLine("A container has been created, note the 'Public access level' in the portal.");
        Console.WriteLine("Press 'Enter' to continue.");
        Console.ReadLine();

        // Create a file in your local MyDocuments folder to upload to a blob.
        string localPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        string localFileName = "BlobDemo_" + Guid.NewGuid().ToString() + ".txt";
        string sourceFile = Path.Combine(localPath, localFileName);
        // Write text to the file.
        File.WriteAllText(sourceFile, "Hello, World!");
        Console.WriteLine("\r\nTemp file = {0}", sourceFile);
        Console.WriteLine("Uploading to Blob storage as blob '{0}'", localFileName);
        // Get a reference to the blob address, then upload the file to the blob.
        // Use the value of localFileName for the blob name.
        CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(localFileName);
        await cloudBlockBlob.UploadFromFileAsync(sourceFile);
        Console.WriteLine("\r\nVerify the creation of the blob and upload in the portal.");
        Console.WriteLine("Press 'Enter' to continue.");
        Console.ReadLine();

        // List the blobs in the container.
        Console.WriteLine("List blobs in container.");
        BlobContinuationToken blobContinuationToken = null;
        do
        {
          var results = await cloudBlobContainer.ListBlobsSegmentedAsync(null, blobContinuationToken);
          // Get the value of the continuation token returned by the listing call.
          blobContinuationToken = results.ContinuationToken;
          foreach (IListBlobItem item in results.Results)
          {
            Console.WriteLine(item.Uri);
          }
        } while (blobContinuationToken != null); // Loop while the continuation token is not null.
        Console.WriteLine("\r\nCompare the list in the console to the portal.");
        Console.WriteLine("Press 'Enter' to continue.");
        Console.ReadLine();

        // Download the blob to a local file, using the reference created earlier.
        // Append the string "_DOWNLOADED" before the .txt extension so that you
        // can see both files in MyDocuments.
        string destinationFile = sourceFile.Replace(".txt", "_DOWNLOADED.txt");
        Console.WriteLine("Downloading blob to {0}", destinationFile);
        await cloudBlockBlob.DownloadToFileAsync(destinationFile, FileMode.Create);
        Console.WriteLine("\r\nLocate the local file to verify it was downloaded.");
        Console.WriteLine("Press 'Enter' to continue.");
        Console.ReadLine();

        // Clean up the resources created by the app
        Console.WriteLine("Press the 'Enter' key to delete the example files and example container.");
        Console.ReadLine();
        // Clean up resources. This includes the container and the two temp files.
        Console.WriteLine("Deleting the container");
        if (cloudBlobContainer != null)
        {
          await cloudBlobContainer.DeleteIfExistsAsync();
        }
        Console.WriteLine("Deleting the source, and downloaded files\r\n");
        File.Delete(sourceFile);
        File.Delete(destinationFile);

        // EXAMPLE CODE ENDS ABOVE HERE
      }
      else
      {
        // Otherwise, let the user know that they need to define the environment variable.
        Console.WriteLine("A valid connection string has not been defined in the storageConnectionString variable.");
        Console.WriteLine("Press enter to exit the application.");
        Console.ReadLine();
      }
    }
  }
}
