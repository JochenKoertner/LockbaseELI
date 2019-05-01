using System;
using System.IO;
using System.Linq;
using System.Net.Mqtt;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Lockbase.CoreDomain;
using Lockbase.CoreDomain.Aggregates;
using Lockbase.CoreDomain.Entities;
using Lockbase.CoreDomain.Services;
using Lockbase.CoreDomain.ValueObjects;
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
				services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
				
				
				// In production, the React files will be served from this directory
				services.AddSpaStaticFiles(spa =>spa.RootPath = "ClientApp" );

				services.AddSingleton<Subject<Statement>>();
				services.AddSingleton(new AtomicValue<LockSystem>(CreateLockSystem()));
				services.AddSingleton<IObservable<Statement>>(sp => sp.GetService<Subject<Statement>>().AsObservable());
				services.AddSingleton<IObserver<Statement>>(sp => sp.GetService<Subject<Statement>>());
				
				services.AddSingleton<IMessageBusInteractor,MessageBusInteractor>();
				
				services.AddMqttService(_configuration);
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
					if (env.IsDevelopment())
					{
						spa.Options.SourcePath = "../../frontend";
						spa.UseReactDevelopmentServer(npmScript: "start");
					}
				});
		}

		private LockSystem CreateLockSystem() => 
			File.ReadAllLines("sample/ELIApp2Drv.txt")
				.Aggregate(LockSystem.Empty, (accu, current) => accu.DefineStatement(current));
	}
}
