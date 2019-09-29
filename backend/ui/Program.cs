using System;
using System.IO;
using ElectronNET.API;
using Lockbase.CoreDomain.ValueObjects;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ui.Common;

namespace ui
{
	public static class Program
	{
		public static void Main(string[] args)
		{	
			var host = CreateHostBuilder(args)
				.Build();
			host.Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
			.ConfigureWebHostDefaults(webBuilder =>
		{
			webBuilder.ConfigureKestrel(serverOptions =>
			{
				serverOptions.ListenLocalhost(5000);
			})
			.UseStartup<Startup>()
			.UseElectron(args);
		});
		
	/* 	public static void Main(string[] args)
		{
			CreateWebHostBuilder(args)
				.Build()
	//			.LoadSample("sample/ELIApp2Drv.txt")
				.Run();
		}


		private static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
				.ConfigureLogging(f => f.AddConsole())
				.UseKestrel(options => {
					options.ListenLocalhost(5000); //HTTP port
				})
				.UseStartup<Startup>()
				.UseElectron(args);

				*/

		private static IHost LoadSample(this IHost host, string fileName)
		{
			var observer = host.Services.GetService<IObserver<Statement>>();
			var brokerConfig = host.Services.GetService<IOptions<BrokerConfig>>().Value;
			
			foreach(var statement in File.ReadAllLines(fileName))
				observer.OnNext(new Statement(brokerConfig.Topic, 007, statement));

			return host;
		}
	}
}

/*
uses 'System.Runtime, Version=4.1.2.0, 
' which has a higher version than referenced assembly 
'System.Runtime' with identity 'System.Runtime, Version=4.1.0.0
 */