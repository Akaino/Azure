using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TeamsWebhook
{

    public class CallRecordModels
    {
        [JsonProperty(PropertyName = "value")]
        public CallRecordModel[] Items { get; set; }
    }

    public class CallRecordModel
    {
        
    // Odata Context.
    [JsonProperty(PropertyName = "@odata.context")]
    public string OdataContext { get; set; }

    // Request Id.
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }

    // Version
    [JsonProperty(PropertyName = "version")]
    public string Version { get; set; }

    // Type
    [JsonProperty(PropertyName = "type")]
    public string Type { get; set; }

    // last modified dateTime
    [JsonProperty(PropertyName = "lastModifiedDateTime")]
    public DateTimeOffset LastModifiedDateTime { get; set; }

    // Call start time
    [JsonProperty(PropertyName = "startDateTime")]
    public DateTimeOffset StartDateTime { get; set; }

    // Call end time
    [JsonProperty(PropertyName = "endDateTime")]
    public DateTimeOffset EndDateTime { get; set; }

    // Join Web URL
    [JsonProperty(PropertyName = "joinWebUrl")]
    public string JoinWebUrl { get; set; }

    // Organizer
    [JsonProperty(PropertyName = "organizer")]
    public JObject Organizer { get; set; }

    // Participants
    [JsonProperty(PropertyName = "participants")]
    public JObject[] Participants { get; set; }

    }
}