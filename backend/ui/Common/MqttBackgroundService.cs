using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Lockbase.CoreDomain.Extensions;
using Lockbase.CoreDomain.Services;
using Lockbase.CoreDomain.ValueObjects;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using MQTTnet.Formatter;
using MQTTnet.Protocol;

namespace ui.Common
{

	public class MqttBackgroundService : BackgroundService
	{
		private readonly BrokerConfig _brokerConfig;
		private readonly ILogger<MqttBackgroundService> _logger;
		private IDisposable _disposables;

		private readonly IMqttClient mqttClient;

		private readonly IObservable<Statement> observableStatement;
		private readonly IObserver<Statement> statementObserver;

		private readonly IObserver<Message> messageObserver;

		public MqttBackgroundService(
			IOptions<BrokerConfig> brokerConfig,
			ILoggerFactory loggerFactory,
			IObservable<Statement> observableStatement,
			IObserver<Statement> statementObserver,
			IObserver<Message> messageObserver)
		{
			_brokerConfig = brokerConfig.Value;
			_logger = loggerFactory.CreateLogger<MqttBackgroundService>();
			this.observableStatement = observableStatement;
			this.statementObserver = statementObserver;
			this.messageObserver = messageObserver;

			// Create a new MQTT client.
			this.mqttClient = new MqttFactory().CreateMqttClient();

			this.mqttClient.UseApplicationMessageReceivedHandler(e => HandleMessage(e.ApplicationMessage));


			// MqttNetGlobalLogger.LogMessagePublished += (sender, e) => _logger.LogInformation(e.LogMessage.ToString());
		}

		private async Task ConnectClient(CancellationToken cancellationToken)
		{
			// Create TCP based options using the builder.
			var options = new MqttClientOptionsBuilder()
				.WithClientId("backend")
				.WithProtocolVersion(MqttProtocolVersion.V500)
				.WithTcpServer(_brokerConfig.HostName, _brokerConfig.Port)
				.WithCleanSession()
				.Build();


			await this.mqttClient.ConnectAsync(options, cancellationToken);
		}

		protected override Task ExecuteAsync(CancellationToken stoppingToken)
		{
			return Task.CompletedTask;
		}

		public override async Task StartAsync(CancellationToken cancellationToken)
		{
			await ConnectClient(cancellationToken);
			_logger.LogInformation($"MQTT Create Client (User:'{_brokerConfig.User}', Topic:'{_brokerConfig.Topic}')");

			var topicFilter = new MqttTopicFilterBuilder()
				.WithTopic(_brokerConfig.Topic)
				.WithAtLeastOnceQoS()
				.Build();

			var subscritionChannel = await this.mqttClient.SubscribeAsync(topicFilter);

			// Hier werden die Antworten zu dem Treiber per 'Publish' verschickt
			var subscriptionStatement = this.observableStatement.Subscribe(
				async statement =>
					await Publish(
						client: mqttClient,
						topic: statement.Topic,
						jobId: statement.JobId,
						payload: statement.Message,
						qos: MqttQualityOfServiceLevel.AtLeastOnce,
						cancellationToken: cancellationToken
					)
			);

			// Hier kommen die Messages vom Treiber an und werden an 'HandleMessage' geroutet


			this._disposables = new CompositeDisposable(
				new[] { subscriptionStatement });

			await base.StartAsync(cancellationToken);
		}

		public override async Task StopAsync(CancellationToken cancellationToken)
		{
			_logger.LogInformation("Stop MQTT-Service");

			await this.mqttClient.DisconnectAsync();

			_disposables?.Dispose();

			await base.StopAsync(cancellationToken);
		}

		// Handelt Messages vom Treiber
		private Task HandleMessage(MqttApplicationMessage msg)
		{
			var correlationId = msg.CorrelationData == null ? null : Encoding.UTF8.GetString(msg.CorrelationData);
			var replyTo = msg.ResponseTopic;
			var message = Encoding.UTF8.GetString(msg.Payload).TrimEnd();

			_logger.LogInformation($"New Message = '{message.Shorten()}', CorrelationId = {correlationId}");

			this.messageObserver.OnNext(new Message { text = message, replyTo = replyTo, correlationId = correlationId.FromHex() });
			return Task.CompletedTask;
		}

		private async Task Publish(IMqttClient client, string topic, int jobId, string payload,
			MqttQualityOfServiceLevel qos, CancellationToken cancellationToken)
		{
			_logger.LogInformation($"publish({topic}, {jobId.ToHex()}, '{payload.Shorten()}')");
			var msg = new MqttApplicationMessage()
			{
				Topic = topic,
				Payload = Encoding.UTF8.GetBytes(payload),
				PayloadFormatIndicator = MqttPayloadFormatIndicator.CharacterData,
				QualityOfServiceLevel = qos,
				CorrelationData = Encoding.UTF8.GetBytes(jobId.ToString("x9"))
			};
			await client.PublishAsync(msg, cancellationToken);
		}
	}
}