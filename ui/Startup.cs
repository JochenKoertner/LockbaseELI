using System.Net.Mqtt;
using Lockbase.CoreDomain;
using Lockbase.CoreDomain.Aggregates;
using Lockbase.CoreDomain.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

using ui.Common; 

namespace ui
{
    public class Startup
    {
        private readonly ILogger<Startup> _logger;

        private readonly IConfiguration _configuration;
        
        public Startup(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            _configuration = configuration;
            _logger = loggerFactory.CreateLogger<Startup>();
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            _logger.LogInformation("ConfigureServices");

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            
            
            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });

            services.AddEventBus(_configuration);

			

			services.AddSingleton(new AtomicValue<LockSystem>(CreateLockSystem()));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            _logger.LogInformation("Configure");
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
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
        }

		private LockSystem CreateLockSystem() {
			return LockSystem.Empty
				.AddKey(new Key(id: "000000hqvs1lo", name: "", appId:"", extData: "MTAzLTEsIEZlbmRlciwgS2xhdXMA"));
/* 
103-1, Fender, Klaus
DK,000000hqvs1lo,,,MTAzLTEsIEZlbmRlciwgS2xhdXMA
DK,040000iavs1lo,,,MTA0LTEsIEtpc3RsZXIsIFNhYmluZQA=
DK,080000ijvs1lo,,,MTA1LTEsIEtvaGwsIFVscmljaAA=
DK,0c0000ml0c25o,,,MjAzLTEsIFdhbHRlciwgSmVucwA=
DK,0g0000ml0c25o,,,MjAzLTIsIFdpbnRlciwgU2luYQA=
DK,0k0000ml0c25o,,,MjAzLTMsIFdvbmRyYXNjaGVrLCBWb2xrZXIA
DK,1s0000l00nuiu,,,MjAwLTEsIExlaW5rYW1wLCBTZWJhc3RpYW4A
DK,200000l00nuiu,,,MjAwLTIA
DK,240000l00nuiu,,,MjAxLTEsIE1lcnRlbnMsIE1hcnRpbmEA
DK,280000l00nuiu,,,MjAxLTIA
DK,2c0000l00nuiu,,,MjAyLTEsIFNpZG93LCBKYW5pbgA=
DK,2g0000l00nuiu,,,MjAyLTIA
DK,7s0000l00nuiu,,,OTAxLTEsIEJhcnRoYXVlciwgVGhvbWFzAA==
DK,840000l00nuiu,,,OTAwLTEsIEFocmVucywgQW5kcmVhAA==
DK,8c0000l00nuiu,,,MjAwLTMA
DK,8g0000l00nuiu,,,MjAwLTQA

DL,000000t00nuiu,,,MTAwLCBNZWV0aW5nIFJvb20sIEFkbWluaXN0cmF0aW9uAA==
DL,040000t00nuiu,,,MTAxLCBPZmZpY2UgQWhyZW5kcywgQWRtaW5pc3RyYXRpb24A
DL,080000t00nuiu,,,MTAyLCBPZmZpY2UgQmFydGhhdWVyLCBBZG1pbmlzdHJhdGlvbgA=
DL,0c0000t00nuiu,,,MTAzLCBBY2NvdW50aW5nLCBBZG1pbmlzdHJhdGlvbgA=
DL,0g0000t00nuiu,,,MTA0LCBPZmZpY2UgU2FsZXMgMSwgQWRtaW5pc3RyYXRpb24A
DL,0k0000t00nuiu,,,MTA1LCBPZmZpY2UgU2FsZXMgMiwgQWRtaW5pc3RyYXRpb24A
DL,1c0000t00nuiu,,,MjAwLCBTdGVlbCBSZXBvc2l0b3J5LCBQcm9kdWN0aW9uAA==
DL,1g0000t00nuiu,,,MjAxLCBQcm9kdWN0IFJlcG9zaXRvcnksIFByb2R1Y3Rpb24A
DL,1k0000t00nuiu,,,MjAyLCBPZmZpY2UgQXNzZW1ibHksIFByb2R1Y3Rpb24A
DL,1s0000t00nuiu,,,MjA0LCBXb3Jrc2hvcCBXZXN0LCBQcm9kdWN0aW9uAA==
DL,200000t00nuiu,,,MjA1LCBXb3Jrc2hvcCBTb3V0aCwgUHJvZHVjdGlvbgA=
DL,240000t00nuiu,,,MjA2LCBHYXRlIFdlc3QsIFByb2R1Y3Rpb24A
DL,580000t00nuiu,,,WjEsIEVudHJhbmNlIFdlc3QsIEFkbWluaXN0cmF0aW9uAA==
*/
		}
    }
}
