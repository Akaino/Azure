using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Linq;
using Microsoft.FeatureManagement;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;

namespace function
{
  public class FeatureFunc
  {
    private readonly IFeatureManagerSnapshot _featureManagerSnapshot;
    private readonly IConfigurationRefresher _configurationRefresher;
    
    public FeatureFunc(IFeatureManagerSnapshot featureManagerSnapshot, IConfigurationRefresherProvider refresherProvider)
    {
      _featureManagerSnapshot = featureManagerSnapshot;
      _configurationRefresher = refresherProvider.Refreshers.First();
    }

    [FunctionName("FeatureFunc")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
        ILogger log)
    {
      log.LogInformation("C# HTTP trigger function processed a request.");

      await _configurationRefresher.TryRefreshAsync();

      string message = await _featureManagerSnapshot.IsEnabledAsync("answerRequests")
              ? "The Feature Flag 'answerRequests' is turned ON"
              : "The Feature Flag 'answerRequests' is turned OFF";

      return (ActionResult)new OkObjectResult(message);
    }
  }
}
