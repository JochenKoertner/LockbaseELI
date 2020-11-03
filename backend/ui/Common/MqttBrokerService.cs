using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Server;

namespace ui.Common
{

	public class MqttBrokerService : BackgroundService
	{
		private readonly BrokerConfig _brokerConfig;
		private readonly ILogger<MqttBrokerService> _logger;
		private IMqttServer _mqttServer;

		public MqttBrokerService(
			IOptions<BrokerConfig> brokerConfig,
			ILoggerFactory loggerFactory)
		{
			_brokerConfig = brokerConfig.Value;
			_logger = loggerFactory.CreateLogger<MqttBrokerService>();
		}

		public override async Task StartAsync(CancellationToken cancellationToken)
		{
			// Server
			_logger.LogInformation($"Start MQTT-Broker (Port:{_brokerConfig.Port})");

			var optionsBuilder = new MqttServerOptionsBuilder()
				.WithDefaultEndpoint()
				.WithDefaultEndpointPort(_brokerConfig.Port)
				.WithSubscriptionInterceptor(
					c =>
					{
						c.AcceptSubscription = true;
						_logger.LogInformation($"New subscription: ClientId = {c.ClientId}, TopicFilter = {c.TopicFilter}");
					}).WithApplicationMessageInterceptor(
					c =>
					{
						c.AcceptPublish = true;
						var payload = c.ApplicationMessage?.Payload == null ? null : Encoding.UTF8.GetString(c.ApplicationMessage?.Payload);
						var correlationId = c.ApplicationMessage?.CorrelationData == null ? null : Encoding.UTF8.GetString(c.ApplicationMessage?.CorrelationData);
						_logger.LogInformation(
				$"Message: ClientId={c.ClientId}, Topic={c.ApplicationMessage?.Topic},"
				+ $" CorrelationId='{correlationId}', ReplyTo='{c.ApplicationMessage?.ResponseTopic}',"
				+ $" Payload='{payload.Substring(0, 10)}...', QoS={c.ApplicationMessage?.QualityOfServiceLevel},"
				+ $" Retain-Flag={c.ApplicationMessage?.Retain}");
					});

			_mqttServer = new MqttFactory().CreateMqttServer();
			await _mqttServer.StartAsync(optionsBuilder.Build());

			await base.StartAsync(cancellationToken);
		}

		public override async Task StopAsync(CancellationToken cancellationToken)
		{
			_logger.LogInformation("Stop MQTT-Broker");
			await _mqttServer.StopAsync();

			await base.StopAsync(cancellationToken);
		}

		protected override Task ExecuteAsync(CancellationToken cancellationToken) => Task.CompletedTask;
	}
}