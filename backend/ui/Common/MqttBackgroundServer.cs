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

	public class MqttBackgroundServer : BackgroundService
	{
		private readonly BrokerConfig _brokerConfig;
		private readonly ILogger<MqttBackgroundServer> _logger;
		private IMqttServer _mqttServer;

		public MqttBackgroundServer(
			IOptions<BrokerConfig> brokerConfig,
			ILoggerFactory loggerFactory)
		{
			_brokerConfig = brokerConfig.Value;
			_logger = loggerFactory.CreateLogger<MqttBackgroundServer>();
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
						_logger.LogInformation(
				$"Message: ClientId = {c.ClientId}, Topic = {c.ApplicationMessage?.Topic},"
				+ $" Payload = {payload}, QoS = {c.ApplicationMessage?.QualityOfServiceLevel},"
				+ $" Retain-Flag = {c.ApplicationMessage?.Retain}");
					});

			_mqttServer = new MqttFactory().CreateMqttServer();
			await _mqttServer.StartAsync(optionsBuilder.Build());

			//_mqttServer = MqttServer.Create(_brokerConfig.Port);
			//_mqttServer.Start();

			await base.StartAsync(cancellationToken);
		}

		public override async Task StopAsync(CancellationToken cancellationToken)
		{
			_logger.LogInformation("Stop MQTT-Broker");
			await _mqttServer.StopAsync();

			await base.StopAsync(cancellationToken);
		}

		protected override Task ExecuteAsync(CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
		}
	}
}