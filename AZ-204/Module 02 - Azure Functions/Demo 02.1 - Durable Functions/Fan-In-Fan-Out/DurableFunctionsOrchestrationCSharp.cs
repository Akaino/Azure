using System.Collections.Generic;
using System.Net.Http;
using System.Linq;
using System.Threading;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;


namespace Function
{
    public static class DurableFunctionsOrchestrationCSharp
    {
        [FunctionName("FanOutInOrchestrator")]
        public static async Task<string> RunOrchestrator(
            [OrchestrationTrigger] DurableOrchestrationContext context, ILogger log)
        {
            log.LogInformation($"************** RunOrchestrator method executing ********************");
 
            GreetingsRequest greetingsRequest = context.GetInput<GreetingsRequest>();
        
            // Fanning out
            log.LogInformation($"************** Fanning out ********************");
            var parallelActivities = new List<Task<string>>();
            foreach (var greeting in greetingsRequest.Greetings)
            {
                // Start a new activity function and capture the task reference
                Task<string> task = context.CallActivityAsync<string>("FanOutIn_ActivityFunction", greeting);
        
                // Store the task reference for later
                parallelActivities.Add(task);
            }
        
            // Wait until all the activity functions have done their work
            log.LogInformation($"************** 'Waiting' for parallel results ********************");
            await Task.WhenAll(parallelActivities);
            log.LogInformation($"************** All activity functions complete ********************");
        
            // Now that all parallel activity functions have completed,
            // fan in AKA aggregate the results, in this case into a single
            // string using a StringBuilder
            log.LogInformation($"************** fanning in ********************");
            var sb = new StringBuilder();
            foreach (var completedParallelActivity in parallelActivities)
            {
                sb.AppendLine(completedParallelActivity.Result);
            }
        
            return sb.ToString();
        }

        [FunctionName("FanOutIn_HttpStart")]
        public static async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")]HttpRequestMessage req,
            [OrchestrationClient]DurableOrchestrationClient starter,
            ILogger log)
        {
            var data = await req.Content.ReadAsAsync<GreetingsRequest>();
 
            string instanceId = await starter.StartNewAsync("FanOutInOrchestrator", data);
        
            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");
        
            return starter.CreateCheckStatusResponse(req, instanceId);
        }

        [FunctionName("FanOutIn_ActivityFunction")]
        public static string SayHello([ActivityTrigger] Greeting greeting, ILogger log)
        {            
            // simulate longer processing delay to demonstrate parallelism
            Thread.Sleep(15000); 
        
            return $"{greeting.Message} {greeting.CityName}";
        }

    }
}