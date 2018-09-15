using System;
using System.Net.Mqtt;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ui
{
    public class Startup
    {
        private readonly IMqttServer mqttServer;
        private readonly ILogger<Startup> logger; 
        
        public Startup(IConfiguration configuration, IMqttServer mqttServer, ILoggerFactory loggerFactory)
        {
            Configuration = configuration;
            this.mqttServer = mqttServer;
            logger = loggerFactory.CreateLogger<Startup>();
            
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            
            
            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });
            
            var applicationLifetime = app.ApplicationServices.GetRequiredService<IApplicationLifetime>();
            applicationLifetime.ApplicationStopping.Register(OnShutdown);
            applicationLifetime.ApplicationStarted.Register(OnStartup);
        }
        

        private void OnShutdown()
        {
            mqttServer.Stop();
            logger.Log(LogLevel.Information, "MQTT-Broker stopped");
        }

        private void OnStartup()
        {
            mqttServer.Start();
            logger.Log(LogLevel.Information, "MQTT-Broker started");
        }
    }
}
