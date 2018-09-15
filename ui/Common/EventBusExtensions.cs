using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Mqtt;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ui.Common.EventBus.Abstractions;
using ui.Common.EventBus.EventBusMQTT;
using ui.Common.EventBus.Events;
using MethodInfo = System.Reflection.MethodInfo;

namespace ui.Common
{
    internal class BrokerConfig
    {
        internal const string KEY = "broker";
        public string HostName { get; private set; } = "localhost";
        public int Port { get; private set; } = 1883;
        public string User { get; private set; } = "Bob";
        public string Topic { get; private set; } = "channel";
    }
        
    internal static class EventBusExtensions
    {
        public static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<BrokerConfig>(configuration.GetSection(BrokerConfig.KEY), options => options.BindNonPublicProperties = true);
            
            services.AddSingleton<IMqttClient>( sp => CreateClient(
                sp.GetRequiredService<BrokerConfig>(), sp.GetRequiredService<ILogger>()));
            
            //services.AddSingleton<IEventBus, EventBusMQTT>();
        //    services.AddHostedService<EventBusListener>();
        
            return services;
        }
        
        private static IMqttClient CreateClient(BrokerConfig brokerConfig, ILogger logger)
        {
            logger.LogInformation("CreateClient");
            var configuration = new MqttConfiguration
            {
                BufferSize = 128 * 1024,
                Port = 1883,
                KeepAliveSecs = 100,
                WaitTimeoutSecs = 2,
                MaximumQualityOfService = MqttQualityOfService.AtMostOnce,
                AllowWildcardsInTopicFilters = true
            };
            
            return MqttClient.CreateAsync(brokerConfig.HostName, configuration).Result;
        }
        
        


    }
}