using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ui.Common
{
    internal static class EventBusExtensions
    {
        public static void AddEventBus(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<BrokerConfig>(configuration.GetSection(BrokerConfig.KEY), 
                options => options.BindNonPublicProperties = true);
            
            services.AddHostedService<MqttBackgroundService>();
        }
    }
}