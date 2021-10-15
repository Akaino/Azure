using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;

using Microsoft.Graph;
using Microsoft.Identity.Client;

using Newtonsoft.Json.Linq;
namespace UploadFile
{
  //POWERSHELL Dummy Files => Set-Content -Path .\File.txt -Value ('.' * 15MB)

  class Program
  {
    static JObject azureConfig = JObject.Parse(System.IO.File.ReadAllText("appSettings.json"));
    static String clientID = azureConfig.GetValue("CLIENT_ID").ToString();
    static String authority = azureConfig.GetValue("AUTHORITY").ToString();
    static String secret = azureConfig.GetValue("SECRET").ToString();

    //Permissions: Files.ReadWrite.All, User.Read.All, User.Read
    private static List<string> scopes = new List<string>();
    private static string fileToUpload = "Small.txt";
    private static string fileToUploadBig = "Big.txt";
    private static string fileToUploadReallyBig = "ReallyBig.txt";

    static void Main(string[] args)
    {
      scopes.Add("https://graph.microsoft.com/.default");

      var ccs = ConfidentialClientApplicationBuilder.Create(clientID)
                                                    .WithAuthority(authority)
                                                    .WithClientSecret(secret)
                                                    .Build();

      GraphServiceClient graphServiceClient = new GraphServiceClient(new DelegateAuthenticationProvider(async (requestMessage) =>
      {
        var authResult = await ccs.AcquireTokenForClient(scopes).ExecuteAsync();

        // Add the access token 
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authResult.AccessToken);
      }));

      var user = graphServiceClient.Users["admin@devtobi.onmicrosoft.com"].Request().GetAsync().GetAwaiter().GetResult();

      UploadSmall(graphServiceClient, fileToUpload, user.Id);

      UploadLarge(graphServiceClient, fileToUploadBig, user.Id);
      UploadLarge(graphServiceClient, fileToUploadReallyBig, user.Id);
    }

    private static void UploadLarge(GraphServiceClient _graphServiceClient, string _fileToUpload, string _userID)
    {
      DriveItem uploadedFile = null;
      FileStream fileStream = new FileStream(_fileToUpload, FileMode.Open);
      UploadSession uploadSession = null;

      uploadSession = _graphServiceClient.Users[_userID]
          .Drive.Root
          .ItemWithPath(_fileToUpload)
          .CreateUploadSession()
          .Request().PostAsync().GetAwaiter().GetResult();

      if (uploadSession != null)
      {
        // Chunk size must be divisible by 320KiB
        int maxSizeChunk = (320 * 1024) * 4;
        ChunkedUploadProvider uploadProvider = new ChunkedUploadProvider(uploadSession, _graphServiceClient, fileStream, maxSizeChunk);

        var chunkRequests = uploadProvider.GetUploadChunkRequests();
        var exceptions = new List<Exception>();
        var readBuffer = new byte[maxSizeChunk];

        foreach (var request in chunkRequests)
        {
          var result = uploadProvider.GetChunkRequestResponseAsync(request, exceptions).GetAwaiter().GetResult();

          Console.WriteLine($"Chunk!");

          if (result.UploadSucceeded)
          {
            uploadedFile = result.ItemResponse;
            Console.WriteLine($"Finished!");
          }
        }

        if (uploadedFile != null)
        {
          Console.WriteLine($"Uploaded file {_fileToUpload} to {uploadedFile.WebUrl}.");
        }
        else
        {
          Console.WriteLine($"Failure uploading {_fileToUpload}");
        }
      }
    }

    private static void UploadSmall(GraphServiceClient _graphServiceClient, string _fileToUpload, string _userID)
    {
      DriveItem uploadedFile = null;
      FileStream fileStream = new FileStream(_fileToUpload, FileMode.Open);

      uploadedFile = _graphServiceClient
          .Users[_userID]
          .Drive
          .Root
          .ItemWithPath(_fileToUpload)
          .Content
          .Request().PutAsync<DriveItem>(fileStream)
          .GetAwaiter().GetResult();

      if (uploadedFile != null)
      {
        Console.WriteLine($"Uploaded file {_fileToUpload} to {uploadedFile.WebUrl}.");
      }
      else
      {
        Console.WriteLine($"Failure uploading {_fileToUpload}");
      }
    }
  }
}
