using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using ElectronNET.API;
using ElectronNET.API.Entities;
using Lockbase.CoreDomain;
using Lockbase.CoreDomain.Aggregates;
using Lockbase.CoreDomain.Services;
using Lockbase.CoreDomain.ValueObjects;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;

using ui.Common;

namespace ui
{
    public class Startup
	{
		readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

		private readonly ILogger<Startup> _logger;

		private readonly IConfiguration _configuration;
		

		public Startup(IWebHostEnvironment env)
		{
			var builder = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json", optional: false)
				.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
				.AddEnvironmentVariables();
			this._configuration = builder.Build();
			// .ConfigureLogging(f => f.AddConsole())
			// 	.UseKestrel(options => {
			// 		options.ListenLocalhost(5000); //HTTP port
			// 	})
			// 	_configuration = configuration;
			// 	_logger = loggerFactory.CreateLogger<Startup>();
		}


		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllersWithViews()
				.AddNewtonsoftJson();
			services.AddRazorPages();
			
			
			services.AddSpaStaticFiles(spa =>spa.RootPath = "ClientApp" );
			// In production, the React files will be served from this directory
			// services.AddSpaStaticFiles(spa =>spa.RootPath = "build");

			services
				.AddSingleton(new AtomicValue<LockSystem>(CreateLockSystem()))
				.AddSingleton<ISubject<Statement>,ReplaySubject<Statement>>()
				.AddSingleton<IObservable<Statement>>(sp => sp.GetService<ISubject<Statement>>().AsObservable())
				.AddSingleton<IObserver<Statement>>(sp => sp.GetService<ISubject<Statement>>())
				.AddSingleton<IMessageBusInteractor,MessageBusInteractor>();

			services.AddCors(options => 
			{
				options.AddPolicy(MyAllowSpecificOrigins, builder =>
				{
					builder// .WithOrigins("http://localhost","http://127.0.0.1")
							.AllowAnyMethod()
							.AllowAnyHeader()
							.AllowAnyOrigin() 
							//.AllowCredentials()
							;
				});
			});

			services
				.AddSignalR(options => { options.KeepAliveInterval = TimeSpan.FromSeconds(5); })
				.AddMessagePackProtocol();
			
			services.AddMqttService(_configuration);
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
					app.UseExceptionHandler("/Error");
				}

				app.UseCors(MyAllowSpecificOrigins);

				app.UseStaticFiles();
				app.UseSpaStaticFiles();
				app.UseRouting();

				app.UseEndpoints(endpoints =>
    			{
        			endpoints.MapHub<SignalrHub>("/signalr");
       			 	endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
    			});

				app.UseSpa(spa =>
				{
					if (env.IsDevelopment())
					{
						spa.Options.SourcePath = "../../frontend";
						spa.UseProxyToSpaDevelopmentServer("http://localhost:3000");
						//spa.UseReactDevelopmentServer(npmScript: "start");
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
