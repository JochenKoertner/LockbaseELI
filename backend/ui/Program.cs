using System;
using System.IO;
//using ElectronNET.API;
using Lockbase.CoreDomain.ValueObjects;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ui
{
	using Common;

	public static class Program
	{
		public static void Main(string[] args)
		{
			CreateHostBuilder(args)
				.Build()
				.Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args)
		=> Host.CreateDefaultBuilder(args)
			.ConfigureWebHostDefaults(webBuilder => webBuilder
				.UseKestrel()
				.UseStartup<Startup>()
			//.UseElectron(args)
			);

	}
}