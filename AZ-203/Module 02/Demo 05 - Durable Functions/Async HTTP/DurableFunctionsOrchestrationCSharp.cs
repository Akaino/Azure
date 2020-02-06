using System.Collections.Generic;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System.Web;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;

namespace Function
{
    public static class DurableFunctionsOrchestrationCSharp
    {
        [FunctionName("AsyncApiPatternExample")]
        public static async Task<string> AsyncApiPatternExample(
            [OrchestrationTrigger] DurableOrchestrationContext context, ILogger log)
        {
            log.LogInformation($"************** RunOrchestrator method executing ********************");
            
            return "London";
        }
        [FunctionName("AsyncApiPatternExample_HttpStartV1")]
        public static async Task<HttpResponseMessage> HttpStartV1(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")]HttpRequestMessage req,
            [OrchestrationClient]DurableOrchestrationClient starter,
            ILogger log)
        {
            string instanceId = await starter.StartNewAsync("AsyncApiPatternExample", null);
        
            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");
        
            return starter.CreateCheckStatusResponse(req, instanceId);
        }

        // tV3COT3TFuxRMEaES/U2wBKqaA/WtbdgVcW/SaKPa2jm0MawaPaFNw==
        [FunctionName("AsyncApiPatternExample_HttpStartV2")]
        public static async Task<HttpResponseMessage> HttpStartV2(
            [HttpTrigger(AuthorizationLevel.Function, "post")]HttpRequestMessage req,
            [OrchestrationClient]DurableOrchestrationClient starter,
            ILogger log)
        {
            string instanceId = await starter.StartNewAsync("AsyncApiPatternExample", null);
        
            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");
        
            // Create the URL to allow the client to check status of a request (excluding the function key in the code querystring)
            string checkStatusUrl = string.Format("{0}://{1}/api/AsyncApiPatternExample_Status?id={2}", req.RequestUri.Scheme, req.RequestUri.Host, instanceId);
        
            // Create the response and add headers
            var response = new HttpResponseMessage()
            {
                StatusCode = System.Net.HttpStatusCode.Accepted,                
                Content = new StringContent(checkStatusUrl),                
            };
            response.Headers.Add("Location", checkStatusUrl);
            response.Headers.Add("Retry-After", "10");
        
            return response;
        }

        [FunctionName("AsyncApiPatternExample_Status")]
        public static async Task<IActionResult> Status(
        [HttpTrigger(AuthorizationLevel.Function, "get")]HttpRequest req,

        [OrchestrationClient]DurableOrchestrationClient orchestrationClient,
        ILogger log)
        {
            var orchestrationInstanceId = req.Query["id"];
        
            if (string.IsNullOrWhiteSpace(orchestrationInstanceId))
            {
                return new NotFoundResult();
            }
        
            // Get the status for the passed in instanceId
            DurableOrchestrationStatus status = await orchestrationClient.GetStatusAsync(orchestrationInstanceId);
        
            if (status is null)
            {
                return new NotFoundResult();
            }
        
            
            var shortStatus = new
            {
                currentStatus = status.RuntimeStatus.ToString(),
                result = status.Output
            };
        
            return new OkObjectResult(shortStatus);
            //  We could also expand this and check status.RuntimeStatus and for example return a 202 if processing is still underway
        }

    }
}