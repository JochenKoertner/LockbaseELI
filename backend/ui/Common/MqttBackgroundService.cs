using System;
using System.Net.Mqtt;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Lockbase.CoreDomain;
using Lockbase.CoreDomain.Extensions;
using Lockbase.CoreDomain.Services;
using Lockbase.CoreDomain.ValueObjects;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
		private IMqttServer _mqttServer;

		private IDisposable _disposables;

		private readonly IMessageBusInteractor messageBusInteractor;
		private readonly IObservable<Statement> observableStatement;
		private readonly IObserver<Statement> statementObserver;

		private readonly IHubContext<SignalrHub, IHubClient> hub;

		public MqttBackgroundService(
			IOptions<BrokerConfig> brokerConfig,
			ILoggerFactory loggerFactory,
			IMessageBusInteractor messageBusInteractor,
			IObservable<Statement> observableStatement,
			IObserver<Statement> statementObserver,
			IHubContext<SignalrHub, IHubClient> hub)
		{
			_brokerConfig = brokerConfig.Value;
			_logger = loggerFactory.CreateLogger<MqttBackgroundService>();
			this.messageBusInteractor = messageBusInteractor;
			this.observableStatement = observableStatement;
			this.statementObserver = statementObserver;
			this.hub = hub;
		}

		private async Task<IMqttClient> CreateClient()
		{
			var configuration = new MqttConfiguration
			{
				BufferSize = 128 * 1024,
				Port = _brokerConfig.Port,
				KeepAliveSecs = 100,
				WaitTimeoutSecs = 2,
				MaximumQualityOfService = MqttQualityOfService.AtMostOnce,
				AllowWildcardsInTopicFilters = true
			};

			return await MqttClient.CreateAsync(_brokerConfig.HostName, configuration);
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			var mqttClient = await CreateClient();

			var sessionState = await Connect(mqttClient, _brokerConfig.User);

			_logger.LogInformation($"MQTT Create Client (User:'{_brokerConfig.User}', Topic:'{_brokerConfig.Topic}')");
			

			await mqttClient.SubscribeAsync(_brokerConfig.Topic, MqttQualityOfService.ExactlyOnce); //QoS2

			// Hier werden die Antworten zu dem Treiber per 'Publish' verschickt
			var subscriptionStatement = this.observableStatement.Subscribe(
				async statement =>
					await Publish(
						client: mqttClient, 
						topic: statement.Topic, 
						sessionId: statement.SessionId, 
						payload: statement.Message,
						replyTo: TOPIC_RESPONSE,
						qos: MqttQualityOfService.ExactlyOnce
					)
			);

			// Hier kommen die Messages vom Treiber an und werden an 'HandleMessage' geroutet
			var subscriptionChannel = mqttClient
				.MessageStream
				.Subscribe(async msg => {
					if (msg.Topic.Equals(_brokerConfig.Topic))
					{
						await HandleMessage(msg);
					}
					else 
						_logger.LogInformation($"Publish {msg.Topic} '{Encoding.UTF8.GetString(msg.Payload)}'");
				});
		
			_disposables = new CompositeDisposable(
				new []{mqttClient, subscriptionChannel, subscriptionStatement});

			while (!stoppingToken.IsCancellationRequested)
			{
				await Task.Delay(5000, stoppingToken);
			}

			//Method to unsubscribe a topic or many topics, which means that the message will no longer
			//be received in the MessageStream anymore
			await mqttClient.UnsubscribeAsync(_brokerConfig.Topic);

			await Disconnect(mqttClient);
		}

		public override Task StartAsync(CancellationToken cancellationToken)
		{
			_logger.LogInformation($"Start MQTT-Broker (Port:{_brokerConfig.Port})");
			_mqttServer = MqttServer.Create(_brokerConfig.Port);
			_mqttServer.Start();
			return base.StartAsync(cancellationToken);
		}

		public override Task StopAsync(CancellationToken cancellationToken)
		{
			_logger.LogInformation("Stop MQTT-Broker");
		
			_disposables?.Dispose();

			_mqttServer.Stop();
			_mqttServer.Dispose();

			return base.StopAsync(cancellationToken);
		}

		// Handelt Messages vom Treiber
		private async Task HandleMessage(MqttApplicationMessage msg)
		{
			var message = JsonConvert.DeserializeObject<Message>(Encoding.UTF8.GetString(msg.Payload));
			messageBusInteractor.Receive(replyTo: message.reply_to, sessionId: message.session_id.FromHex(), message: message.text);

			var signalrMsg = new MessageInstance()
				{ Timestamp = DateTime.UtcNow.ToString(), From= message.session_id, Message = message.text };
			await this.hub.Clients.All.BroadcastMessage(signalrMsg);
		}

		private async Task<SessionState> Connect(IMqttClient client, string clientId)
		{
			var sessionState = await client.ConnectAsync(new MqttClientCredentials(clientId), cleanSession: true);
			return sessionState;
		}

		private async Task Disconnect(IMqttClient client)
		{
			await client.DisconnectAsync();
		}

		private async Task Publish(IMqttClient client, string topic, int sessionId, string payload, string replyTo, MqttQualityOfService qos)
		{
			_logger.LogInformation($"Topic: '{topic}', Session:{sessionId}, '{payload}'");
			var message = new Message()
			{
				session_id = sessionId.ToString("X8"),
				text = payload,
				reply_to = replyTo
			};
			var json = JsonConvert.SerializeObject(message, Formatting.Indented);
			var msg = new MqttApplicationMessage(topic, Encoding.UTF8.GetBytes(json));
			await client.PublishAsync(msg, qos);
		}
	}
}