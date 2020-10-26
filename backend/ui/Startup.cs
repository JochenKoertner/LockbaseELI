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
using Lockbase.ui.Resources;
using Lockbase.CoreDomain.Contracts;
using Microsoft.AspNetCore.SignalR;

namespace ui
{
    public class Startup
	{
		readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

		private readonly IConfiguration configuration;
		

		public Startup(IConfiguration configuration)
		{
			this.configuration = configuration;
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
				.AddSingleton<IDateTimeProvider>(new DateTimeProvider())
				.AddSingleton<Id>(sp => new Id(sp.GetService<IDateTimeProvider>()))
				.AddSingleton(sp => new AtomicValue<LockSystem>(LockSystem.Create(sp.GetService<Id>())))

                .AddSingleton<ISubject<Statement>,ReplaySubject<Statement>>()
				.AddSingleton<IObservable<Statement>>(sp => sp.GetService<ISubject<Statement>>().AsObservable())
				.AddSingleton<IObserver<Statement>>(sp => sp.GetService<ISubject<Statement>>())

                .AddSingleton<IMessageBusInteractor,MessageBusInteractor>()

                .AddSingleton<ISubject<Message>, ReplaySubject<Message>>()
                .AddSingleton<IObservable<Message>>(sp => sp.GetService<ISubject<Message>>().AsObservable())
                .AddSingleton<IObserver<Message>>(sp => sp.GetService<ISubject<Message>>())

                .AddSingleton<BrowserChannel>(sp => new BrowserChannel(
                   sp.GetService<IObservable<Message>>(),
                   sp.GetService<IHubContext<SignalrHub, IHubClient>>(),
                   sp.GetService<ILoggerFactory>(),
                   sp.GetService<IDateTimeProvider>()));

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

            services
                .AddMqttService(this.configuration);
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
            app.ApplicationServices.GetService<BrowserChannel>();

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
	}
}
