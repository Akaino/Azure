using System;
using Microsoft.Azure.ServiceBus;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.IO;

namespace ServiceBus
{
    class Program
    {
        static JObject azureConfig = JObject.Parse(File.ReadAllText(@"C:\Users\Kai.Roth\Documents\Development\Azure\AZ-204\Module 11 - Develop message-based solutions\Service Bus\ServiceBus\ServiceBus\azureConfig.json"));

        // Batch Account credentials
        static String ServiceBusConnectionString = azureConfig.GetValue("ServiceBusConnectionString").ToString();
        const string QueueName = "myqueue";
        const string TopicName = "mytopic";

        static IQueueClient queueClient;
        static ITopicClient topicClient;
        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {
            const int numberOfMessages = 10;
            queueClient = new QueueClient(ServiceBusConnectionString, QueueName);
            topicClient = new TopicClient(ServiceBusConnectionString, TopicName);

            Console.WriteLine("======================================================");
            Console.WriteLine("Press ENTER key to exit after sending all the messages.");
            Console.WriteLine("======================================================");
            
            // Send messages.
            //await SendMessagesAsync(numberOfMessages);
            while (Console.ReadLine() != "quit")
            {
                await SendMessagesAsync(numberOfMessages);
                Message m = new Message(Encoding.UTF8.GetBytes("MyBody"));
                m.Label = "myLabel";
                await queueClient.SendAsync(m);
            }
            await topicClient.CloseAsync();
            //await queueClient.CloseAsync();
        }

        static async Task SendMessagesAsync(int numberOfMessagesToSend)
        {
            try
            {
                for (var i = 0; i < numberOfMessagesToSend; i++)
                {
                    // Create a new message to send to the queue.
                    string messageBody = $"Message {i}";
                    var message = new Message(Encoding.UTF8.GetBytes(messageBody));

                    // Write the body of the message to the console.
                    Console.WriteLine($"Sending message: {messageBody}");

                    // Send the message to the queue.
                    await topicClient.SendAsync(message);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine($"{DateTime.Now} :: Exception: {exception.Message}");
            }
        }
    }
}
