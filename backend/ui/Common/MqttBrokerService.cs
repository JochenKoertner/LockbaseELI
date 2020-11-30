using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Lockbase.CoreDomain.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client.Receiving;
using MQTTnet.Server;

namespace ui.Common
{

	public class MqttBrokerService : BackgroundService,
		IMqttServerClientConnectedHandler,
		IMqttServerClientDisconnectedHandler,
		IMqttApplicationMessageReceivedHandler
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
					})
				.WithApplicationMessageInterceptor(
					c =>
					{
						c.ApplicationMessage.Payload = 
							Encoding.UTF8.GetBytes(
								Encoding.UTF8.GetString(c.ApplicationMessage.Payload).Trim());
						//c.ApplicationMessage.Payload = Encoding.UTF8.GetBytes(payload);
					});

			_mqttServer = new MqttFactory().CreateMqttServer();

			_mqttServer.ClientConnectedHandler = this;
			_mqttServer.ClientDisconnectedHandler = this;
			_mqttServer.ApplicationMessageReceivedHandler = this;
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
		public Task HandleClientConnectedAsync(MqttServerClientConnectedEventArgs eventArgs)
		{
			_logger.LogInformation($"Client Connect {eventArgs.ClientId}");
			return Task.CompletedTask;
		}

		public Task HandleClientDisconnectedAsync(MqttServerClientDisconnectedEventArgs eventArgs)
		{
			_logger.LogInformation($"Client Disconnect {eventArgs.ClientId} , {eventArgs.DisconnectType}");
			return Task.CompletedTask;
		}

		public Task HandleApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs eventArgs)
		{
			_logger.LogInformation($"Message Receive {eventArgs.ClientId}, {eventArgs.ApplicationMessage.Topic}, {eventArgs.ProcessingFailed}, {Encoding.UTF8.GetString(eventArgs.ApplicationMessage.Payload).Shorten()}");
			return Task.CompletedTask;
		}
	}
}