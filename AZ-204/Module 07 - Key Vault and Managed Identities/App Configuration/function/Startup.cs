using System;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.FeatureManagement;

[assembly: FunctionsStartup(typeof(FunctionApp.Startup))]

namespace FunctionApp
{
  class Startup : FunctionsStartup
  {
    public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
    {
      builder.ConfigurationBuilder.AddAzureAppConfiguration(options =>
      {
        options.Connect(Environment.GetEnvironmentVariable("ConnectionString"))
             .Select("App1:*")
             // Configure to reload configuration if the registered 'Sentinel' key is modified
             .ConfigureRefresh(refreshOptions =>
                refreshOptions.Register("App1:Refreshing", refreshAll: true)
             )
             .UseFeatureFlags();
      });
    }

    public override void Configure(IFunctionsHostBuilder builder)
    {
      builder.Services.AddAzureAppConfiguration();
      builder.Services.AddFeatureManagement();
    }
  }
}