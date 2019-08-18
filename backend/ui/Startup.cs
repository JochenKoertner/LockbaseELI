using System;
using System.IO;
using System.Linq;
using System.Net.Mqtt;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using ElectronNET.API;
using ElectronNET.API.Entities;
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

				services
					.AddSingleton(new AtomicValue<LockSystem>(CreateLockSystem()))
					.AddSingleton<ISubject<Statement>,ReplaySubject<Statement>>()
					.AddSingleton<IObservable<Statement>>(sp => sp.GetService<ISubject<Statement>>().AsObservable())
					.AddSingleton<IObserver<Statement>>(sp => sp.GetService<ISubject<Statement>>())
					.AddSingleton<IMessageBusInteractor,MessageBusInteractor>();

				services.AddCors(o => o.AddPolicy("CorsPolicy", b =>
				{
					b.AllowAnyMethod()
						.AllowAnyHeader()
						.AllowAnyOrigin()
						.AllowCredentials();
				}));

				services
					.AddSignalR(options => { options.KeepAliveInterval = TimeSpan.FromSeconds(5); })
					.AddMessagePackProtocol();
				
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
				}

				app.UseStaticFiles();
				app.UseSpaStaticFiles();
				app.UseCors("CorsPolicy");

				app.UseSignalR(routes =>
				{
					routes.MapHub<SignalrHub>("/signalr");
				});

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

				// Open the Electron-Window here
				Task.Run(async () => await Electron.WindowManager.CreateWindowAsync(
					new BrowserWindowOptions
					{
						Width = 1260,
						Height = 1200,
					}));
		}

		private LockSystem CreateLockSystem() => LockSystem.Empty;
		//	File.ReadAllLines("sample/ELIApp2Drv.txt")
		//		.Aggregate(LockSystem.Empty, (accu, current) => accu.DefineStatement(current));
	}
}
