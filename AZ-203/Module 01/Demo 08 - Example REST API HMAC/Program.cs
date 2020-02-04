using System;
using System.Net;
using System.Threading;
using System.IO;

using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Demo_08___Example_REST_API_HMAC
{
    class Program
    {
        static JObject azureConfig = JObject.Parse(File.ReadAllText("azureConfig.json"));

        // Batch Account credentials
        

        static void Main(string[] args)
        {
            AsyncMain(args).GetAwaiter().GetResult();
        }
        static async Task AsyncMain(string[] args) {
            String storageAccount = azureConfig.GetValue("StorageAccountName").ToString();
            String accessKey = azureConfig.GetValue("StorageAccountKey").ToString();
            String resourcePath = "?comp=list";
            String uri = @"Https://" + storageAccount + ".file.core.windows.net/" + resourcePath;

            // Web request 
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);
            request.Method = "GET";
            request.Headers["x-ms-date"] = DateTime.UtcNow.ToString("R", System.Globalization.CultureInfo.InvariantCulture);
            request.Headers["x-ms-version"] = "2015-02-21";
            String stringToSign = "GET\n"
                + "\n" // content encoding
                + "\n" // content language
                + "\n" // content length
                + "\n" // content md5
                + "\n" // content type
                + "\n" // date
                + "\n" // if modified since
                + "\n" // if match
                + "\n" // if none match
                + "\n" // if unmodified since
                + "\n" // range
                + "x-ms-date:" + request.Headers["x-ms-date"] 
                + "\n" //
                +"x-ms-version:2015-02-21\n" // headers
                + "/" + storageAccount + "/" + "\ncomp:list"; // resources

            System.Security.Cryptography.HMACSHA256 hasher = new System.Security.Cryptography.HMACSHA256(Convert.FromBase64String(accessKey));
            string strAuthorization = "SharedKey " + storageAccount + ":" + System.Convert.ToBase64String(hasher.ComputeHash(System.Text.Encoding.UTF8.GetBytes(stringToSign)));

            request.Headers["Authorization"] = strAuthorization;

            WebResponse response = await request.GetResponseAsync();
            
            using (System.IO.StreamReader r = new System.IO.StreamReader(response.GetResponseStream()))
            {
                string jsonData = r.ReadToEnd();
                Console.WriteLine("Reading JSON");
                Console.WriteLine(jsonData);
            }

        }
    }
}
