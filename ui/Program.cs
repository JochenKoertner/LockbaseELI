using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mqtt;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace ui
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();
            host.Run();
        }


        private static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureLogging(f => f.AddConsole())
                .ConfigureServices( services => 
                    services.TryAdd(ServiceDescriptor.Singleton<IMqttServer>(provider => MqttServer.Create(port:1883))))
                .UseStartup<Startup>();
    }
}
