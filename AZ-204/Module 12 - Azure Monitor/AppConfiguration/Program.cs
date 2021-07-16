using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace AppConfiguration
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
          Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
              webBuilder.ConfigureAppConfiguration((hostingContext, config) =>
              {
                  var settings = config.Build();
                  // #AppConfiguration
                  config.AddAzureAppConfiguration(options =>
                  {
                      options.Connect(settings["ConnectionStrings:AppConfig"])
                        .ConfigureRefresh(refresh =>
                          {
                            // Set Configuration Refresh to listen to Sentinel Key.
                            // No Change to sentinel key? No Configuration Refresh!
                            refresh.Register("CSS:Settings:Sentinel", refreshAll: true)
                                  .SetCacheExpiration(new TimeSpan(0, 0, 5)); //Overwrite default of 30s
                          });
                  });
              })
          .UseStartup<Startup>());
    }
}
