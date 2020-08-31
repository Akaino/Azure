using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace Function
{
    public static class MonitorPatternExample
    {
        [FunctionName("MonitorPatternExample")]
        public static async Task RunOrchestrator(
        [OrchestrationTrigger] DurableOrchestrationContext context, ILogger log)
        {
            string fileName = context.GetInput<string>();
        
            // start encoding
            await context.CallActivityAsync<string>("MonitorPatternExample_BeginEncode", fileName);

            // We don't want the orchestration to run infinitely
            // If the operation has not completed within 30 mins, end the orchestration
            var operationTimeoutTime = context.CurrentUtcDateTime.AddMinutes(30);
        
            while (true)
            {
                var operationHasTimedOut = context.CurrentUtcDateTime > operationTimeoutTime;
        
                if (operationHasTimedOut)
                {
                    context.SetCustomStatus("Encoding has timed out, please submit the job again.");
                    break;
                }
        
                var isEncodingComplete = await context.CallActivityAsync<bool>("MonitorPatternExample_IsEncodingComplete", fileName);
        
                if (isEncodingComplete)
                {
                    context.SetCustomStatus("Encoding has completed successfully.");
                    break;
                }
        
                // If no timeout and encoding still being processed we want to put the orchestration to sleep,
                // and awaking it again after a specified interval
                var nextCheckTime = context.CurrentUtcDateTime.AddSeconds(15);
                log.LogInformation($"************** Sleeping orchestration until {nextCheckTime.ToLongTimeString()}");
                await context.CreateTimer(nextCheckTime, CancellationToken.None);
            }
        }

        [FunctionName("MonitorPatternExample_HttpStart")]
        public static async Task<HttpResponseMessage> HttpStartV1(
            [HttpTrigger(AuthorizationLevel.Function, "post")]HttpRequestMessage req,
            [OrchestrationClient]DurableOrchestrationClient starter,
            ILogger log)
        {            
            dynamic data = await req.Content.ReadAsAsync<dynamic>();
            var fileName = data.FileName;
        
            string instanceId = await starter.StartNewAsync("MonitorPatternExample", fileName);
        
            return starter.CreateCheckStatusResponse(req, instanceId);
        }

        [FunctionName("MonitorPatternExample_BeginEncode")]
        public static void BeginEncodeVideo([ActivityTrigger] string fileName, ILogger log)
        {
            // Call API, start an async process, queue a message, etc.
            log.LogInformation($"************** Starting encoding of {fileName}");
 
            // This activity returns before the job is complete, its job is to just start the async/long running operation
        }
 
 
        [FunctionName("MonitorPatternExample_IsEncodingComplete")]
        public static bool IsEncodingComplete([ActivityTrigger] string fileName, ILogger log)
        {
            log.LogInformation($"************** Checking if {fileName} encoding is complete...");
            // Here you would make a call to an API, query a database, check blob storage etc 
            // to check whether the long running asyn process is complete
 
            // For demo purposes, we'll just signal completion every so often
            bool isComplete = new Random().Next() % 2 == 0;
 
            log.LogInformation($"************** {fileName} encoding complete: {isComplete}");
 
            return isComplete;
        }

    }
}