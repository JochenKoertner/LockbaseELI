using System;
using System.IO;
using ElectronNET.API;
using Lockbase.CoreDomain.ValueObjects;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ui.Common;

namespace ui
{
	public static class Program
	{
		public static void Main(string[] args)
		{
			CreateWebHostBuilder(args)
				.Build()
	//			.LoadSample("sample/ELIApp2Drv.txt")
				.Run();
		}


		private static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
				.ConfigureLogging(f => f.AddConsole())
				.UseStartup<Startup>()
				.UseElectron(args);

		private static IWebHost LoadSample(this IWebHost host, string fileName)
		{
			var observer = host.Services.GetService<IObserver<Statement>>();
			var brokerConfig = host.Services.GetService<IOptions<BrokerConfig>>().Value;
			
			foreach(var statement in File.ReadAllLines(fileName))
				observer.OnNext(new Statement(brokerConfig.Topic, 007, statement));

			return host;
		}
	}
}
