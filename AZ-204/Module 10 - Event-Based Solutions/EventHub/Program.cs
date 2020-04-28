using System;
using System.Text;
using Microsoft.Azure.EventHubs;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;
using System.IO;

namespace EventHub
{
    class Program
    {
        private static EventHubClient eventHubClient;
        static JObject azureConfig = JObject.Parse(File.ReadAllText("azureConfig.json"));
        static String EventHubConnectionString = azureConfig.GetValue("EventHubConnectionString").ToString();
        private const string EventHubName = "workshophub";
        static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }

        private static async Task MainAsync(string[] args)
        {
            // Creates an EventHubsConnectionStringBuilder object from the connection string, and sets the EntityPath.
            // Typically, the connection string should have the entity path in it, but this simple scenario
            // uses the connection string from the namespace.
            var connectionStringBuilder = new EventHubsConnectionStringBuilder(EventHubConnectionString)
            {
                EntityPath = EventHubName
            };

            eventHubClient = EventHubClient.CreateFromConnectionString(connectionStringBuilder.ToString());
            
            await SendMessagesToEventHub(100);

            await eventHubClient.CloseAsync();

            Console.WriteLine("Press ENTER to exit.");
            Console.ReadLine();
        }
        // Uses the event hub client to send 100 messages to the event hub.
        private static async Task SendMessagesToEventHub(int numMessagesToSend)
        {
            for (var i = 0; i < numMessagesToSend; i++)
            {
                try
                {
                    var message = $"Message {i}";
                    
                    Console.WriteLine($"Sending message: {message}");
                    var data = new EventData(Encoding.UTF8.GetBytes(message));
                    
                    await eventHubClient.SendAsync(data);
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"{DateTime.Now} > Exception: {exception.Message}");
                }

                await Task.Delay(10);
            }

            Console.WriteLine($"{numMessagesToSend} messages sent.");
        }
    }
}