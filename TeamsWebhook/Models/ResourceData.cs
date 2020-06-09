using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TeamsWebhook.Models
{
  public class ResourceData
  {
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }

    [JsonProperty(PropertyName = "@odata.etag")]
    public string ODataEtag { get; set; }

    // The OData ID of the resource. This is the same value as the resource property.
    [JsonProperty(PropertyName = "@odata.id")]
    public string ODataId { get; set; }

    // // The OData type of the resource: "#Microsoft.Graph.Message", "#Microsoft.Graph.Event", or "#Microsoft.Graph.Contact".
    [JsonProperty(PropertyName = "@odata.type")]
    public string ODataType { get; set; }


    
    // [JsonProperty(PropertyName = "endDateTime")]
    // public string EndDateTime { get; set; }

    // [JsonProperty(PropertyName = "joinWebUrl")]
    // public string JoinWebUrl { get; set; }

    // [JsonProperty(PropertyName = "lastModifiedDateTime")]
    // public string LastModifiedDateTime { get; set; }
    
    // [JsonProperty(PropertyName = "modalities")]
    // public string[] Modalities { get; set; }
        
    // [JsonProperty(PropertyName = "organizer")]
    // public JObject Organizer { get; set; }
    
    // [JsonProperty(PropertyName = "participants")]
    // public JObject[] Participants { get; set; }
    
    // [JsonProperty(PropertyName = "startDateTime")]
    // public string StartDateTime { get; set; }
     
    // [JsonProperty(PropertyName = "type")]
    // public string Type { get; set; }
         
    // [JsonProperty(PropertyName = "version")]
    // public System.Int64 Version { get; set; }
    // // The OData etag property.
  }
}