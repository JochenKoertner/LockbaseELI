using System;
// using System.Net.Mqtt;
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
using MQTTnet.Protocol;
using Newtonsoft.Json;

namespace ui.Common
{
	public class Message
	{
		public string session_id { get; set; }
		public string text { get; set; }
		public string reply_to { get; set; }
	}

	public class MqttBackgroundService : BackgroundService
	{
		private const string TOPIC_RESPONSE = "response";
		private readonly BrokerConfig _brokerConfig;
		private readonly ILogger<MqttBackgroundService> _logger;
		private IDisposable _disposables;

		private readonly IMqttClient mqttClient;

		private readonly IMessageBusInteractor messageBusInteractor;
		private readonly IObservable<Statement> observableStatement;
		private readonly IObserver<Statement> statementObserver;

		private readonly IObserver<Message> messageObserver;

		public MqttBackgroundService(
			IOptions<BrokerConfig> brokerConfig,
			ILoggerFactory loggerFactory,
			IMessageBusInteractor messageBusInteractor,
			IObservable<Statement> observableStatement,
			IObserver<Statement> statementObserver,
			IObserver<Message> messageObserver)
		{
			_brokerConfig = brokerConfig.Value;
			_logger = loggerFactory.CreateLogger<MqttBackgroundService>();
			this.messageBusInteractor = messageBusInteractor;
			this.observableStatement = observableStatement;
			this.statementObserver = statementObserver;
			this.messageObserver = messageObserver;

			// Create a new MQTT client.
			this.mqttClient = new MqttFactory().CreateMqttClient();
		}

		private async Task ConnectClient(CancellationToken cancellationToken)
		{
			// Create TCP based options using the builder.
			var options = new MqttClientOptionsBuilder()
				.WithClientId("backend")
				.WithCredentials(username: _brokerConfig.User, password: (string)null)
				.WithTcpServer(_brokerConfig.HostName, _brokerConfig.Port)
				.WithCommunicationTimeout(TimeSpan.FromSeconds(2))
				.WithKeepAlivePeriod(TimeSpan.FromSeconds(10))
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

			await this.mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic(_brokerConfig.Topic).Build());

			// Hier werden die Antworten zu dem Treiber per 'Publish' verschickt
			var subscriptionStatement = this.observableStatement.Subscribe(
				async statement =>
					await Publish(
						client: mqttClient,
						topic: statement.Topic,
						sessionId: statement.SessionId,
						payload: statement.Message,
						//replyTo: TOPIC_RESPONSE,
						qos: MqttQualityOfServiceLevel.AtMostOnce,
						cancellationToken: cancellationToken
					)
			);

			// Hier kommen die Messages vom Treiber an und werden an 'HandleMessage' geroutet
			this.mqttClient.UseApplicationMessageReceivedHandler(e =>
			{
				HandleMessage(e.ApplicationMessage);
			});


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
		private void HandleMessage(MqttApplicationMessage msg)
		{
			var message = JsonConvert.DeserializeObject<Message>(Encoding.UTF8.GetString(msg.Payload));
			messageBusInteractor.Receive(replyTo: message.reply_to, sessionId: message.session_id.FromHex(), message: message.text);

			this.messageObserver.OnNext(message);
		}

		private async Task Publish(IMqttClient client, string topic, int sessionId, string payload,
			MqttQualityOfServiceLevel qos, CancellationToken cancellationToken)
		{
			_logger.LogInformation($"Topic: '{topic}', Session:{sessionId}, '{payload}'");
			var message = new Message()
			{
				session_id = sessionId.ToString("X8"),
				text = payload
			};
			var json = JsonConvert.SerializeObject(message, Formatting.Indented);

			var msg = new MqttApplicationMessage()
			{
				Topic = topic,
				Payload = Encoding.UTF8.GetBytes(json),
				QualityOfServiceLevel = qos
			};
			await client.PublishAsync(msg, cancellationToken);
		}
	}
}