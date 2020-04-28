using System;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using System.Collections.Generic;


namespace EventGrid
{
    class Program
    {
        static void Main(string[] args)
        {
            string topicOrDomainEndpoint = "https://topicOrDomainName.northeurope-1.eventgrid.azure.net/api/events";
            string topicOrDomainKey = "<KEY>";
            string topicOrDomainHostname = new Uri(topicOrDomainEndpoint).Host;

            TopicCredentials topicCredentials = new TopicCredentials(topicOrDomainKey);
            EventGridClient client = new EventGridClient(topicCredentials);
            
            client.PublishEventsAsync(topicOrDomainHostname, GetEventsList()).GetAwaiter().GetResult();
            Console.Write("Published events to Event Grid.");

            static IList<EventGridEvent> GetEventsList()
            {
                List<EventGridEvent> eventsList = new List<EventGridEvent>();
                for (int i = 0; i < 1; i++)
                {
                    eventsList.Add(new EventGridEvent()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Topic = "<domaintopic>",
                        EventType = "My.Custom.Event.Type",
                        Data = new Event() 
                        {
                            ItemInformation = "Some information",
                            Schulung = "askjdasd"
                        },

                        EventTime = DateTime.Now,
                        Subject = "My custom subject",
                        DataVersion = "2.0"
                    });
                }
                return eventsList;
            }
        }
    }
}
