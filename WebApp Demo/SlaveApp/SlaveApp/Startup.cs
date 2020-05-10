using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// TelemetryInitializer
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace SlaveApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Application Insights Options
            Microsoft.ApplicationInsights.AspNetCore.Extensions.ApplicationInsightsServiceOptions aiOptions
                = new Microsoft.ApplicationInsights.AspNetCore.Extensions.ApplicationInsightsServiceOptions();
            
            // Disables adaptive sampling.
            aiOptions.EnableAdaptiveSampling = true;
            // Disables QuickPulse (Live Metrics stream).
            aiOptions.EnableQuickPulseMetricStream = true;
            /*
                EnablePerformanceCounterCollectionModule 
                    PerformanceCounterCollectionModule aktivieren / deaktivieren  
                        true
                EnableRequestTrackingTelemetryModule 
                    RequestTrackingTelemetryModule aktivieren / deaktivieren  
                        true
                EnableEventCounterCollectionModule 
                    EventCounterCollectionModule aktivieren / deaktivieren    
                        true
                EnableDependencyTrackingTelemetryModule 
                    DependencyTrackingTelemetryModule aktivieren / deaktivieren   
                        true
                EnableAppServicesHeartbeatTelemetryModule 
                    AppServicesHeartbeatTelemetryModule aktivieren / deaktivieren 
                        true
                EnableAzureInstanceMetadataTelemetryModule 
                    AzureInstanceMetadataTelemetryModule aktivieren / deaktivieren    
                        true
                EnableQuickPulseMetricStream    
                    „LiveMetrics“-Feature aktivieren / deaktivieren   
                        true
                EnableAdaptiveSampling 
                    Adaptive Stichprobenerstellung aktivieren/ deaktivieren  
                        true
                EnableHeartbeat 
                    Heartbeats-Feature aktivieren / deaktivieren, das in regelmäßigen Abständen(Standardwert: 15 Minuten) 
                    eine benutzerdefinierte Metrik namens „HeartBeatState“ mit Informationen zur Laufzeit wie.NET - Version, 
                    ggf.Informationen zur Azure - Umgebung usw.sendet. 
                        true
                AddAutoCollectedMetricExtractor 
                    Extraktor für „AutoCollectedMetrics“ aktivieren / deaktivieren, bei dem es sich um einen 
                    Telemetrieprozessor handelt, der vorab aggregierte Metriken zu Anforderungen / Abhängigkeiten sendet, 
                    bevor die Stichprobenerstellung stattfindet.	
                        true
                RequestCollectionOptions.TrackExceptions 
                    Berichterstellung über die Nachverfolgung von Ausnahmefehlern durch das Anforderungserfassungsmodul aktivieren / deaktivieren.
                        In NETSTANDARD2.0 „false“ (da Ausnahmen mit „ApplicationInsightsLoggerProvider“ nachverfolgt werden), andernfalls „true“.

            */
            services.AddSingleton<ITelemetryInitializer, MyTelemetryInitializer>();
            services.AddApplicationInsightsTelemetry(aiOptions);
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        public class MyTelemetryInitializer : ITelemetryInitializer
        {
            public void Initialize(ITelemetry telemetry)
            {
                var requestTelemetry = telemetry as RequestTelemetry;
                // Is this a TrackRequest() ?
                if (requestTelemetry == null) return;
                int code;
                bool parsed = Int32.TryParse(requestTelemetry.ResponseCode, out code);
                if (!parsed) return;
                if (code >= 400 && code < 500)
                {
                    // If we set the Success property, the SDK won't change it:
                    requestTelemetry.Success = true;

                    // Allow us to filter these requests in the portal:
                    requestTelemetry.Properties["Overridden400s"] = "true";
                }
                // else leave the SDK to set the Success property
            }
        }
    }
}
