using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ui.Common
{
	internal static class EventBusExtensions
	{
		public static IServiceCollection AddMqttService(this IServiceCollection services, IConfiguration configuration)
		{
			services.Configure<BrokerConfig>(configuration.GetSection(BrokerConfig.KEY), 
				options => options.BindNonPublicProperties = true);
			
			return services
				.AddHostedService<MqttBrokerService>()
				.AddHostedService<MqttBackgroundService>();
		}
	}
}
