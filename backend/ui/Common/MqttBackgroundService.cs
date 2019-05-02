using System;
using System.Net.Mqtt;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Lockbase.CoreDomain;
using Lockbase.CoreDomain.Services;
using Lockbase.CoreDomain.ValueObjects;
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
		private const string TOPIC_HEARTBEAT = "heartbeat";
		private readonly BrokerConfig _brokerConfig;
		private readonly ILogger<MqttBackgroundService> _logger;
		private IMqttServer _mqttServer;

		private IDisposable _disposables;

		private readonly IMessageBusInteractor messageBusInteractor;
		private readonly IObservable<Statement> observableStatement;
		private readonly IObserver<Statement> statementObserver;

		public MqttBackgroundService(
			IOptions<BrokerConfig> brokerConfig,
			ILoggerFactory loggerFactory,
			IMessageBusInteractor messageBusInteractor,
			IObservable<Statement> observableStatement,
			IObserver<Statement> statementObserver)
		{
			_brokerConfig = brokerConfig.Value;
			_logger = loggerFactory.CreateLogger<MqttBackgroundService>();
			this.messageBusInteractor = messageBusInteractor;
			this.observableStatement = observableStatement;
			this.statementObserver = statementObserver;
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

			await mqttClient.SubscribeAsync(_brokerConfig.Topic, MqttQualityOfService.ExactlyOnce); //QoS2
			await mqttClient.SubscribeAsync(TOPIC_RESPONSE, MqttQualityOfService.ExactlyOnce); //QoS2

			var subscriptionStatement = this.observableStatement.Subscribe(
				async statement =>
					await Publish(
						client: mqttClient, 
						topic: statement.Topic, 
						sessionId: statement.SessionId, 
						payload: statement.Message,
						replyTo: TOPIC_RESPONSE,
						qos: statement.Topic.Equals(TOPIC_HEARTBEAT) ? 
							MqttQualityOfService.AtMostOnce 
							: 
							MqttQualityOfService.ExactlyOnce
					)
			);

			var subscriptionChannel = mqttClient
				.MessageStream
				.Subscribe(msg => {
					if (msg.Topic.Equals(_brokerConfig.Topic))
						HandleMessage(msg);
					else 
						_logger.LogInformation($"Publish {msg.Topic} '{Encoding.UTF8.GetString(msg.Payload)}'");
				});
		
			_disposables = new CompositeDisposable(
				new []{mqttClient, subscriptionChannel, subscriptionStatement});

			while (!stoppingToken.IsCancellationRequested)
			{
				// all 30sec do some lookup for working 
				this.statementObserver.OnNext(new Statement(TOPIC_HEARTBEAT, 4711, $"HC,{DateTime.UtcNow.ToShortTimeString()}"));
				await Task.Delay(5000, stoppingToken);
			}

			//Method to unsubscribe a topic or many topics, which means that the message will no longer
			//be received in the MessageStream anymore
			await mqttClient.UnsubscribeAsync(_brokerConfig.Topic);

			await Disconnect(mqttClient);
		}

		public override Task StartAsync(CancellationToken cancellationToken)
		{
			_logger.LogInformation("Start MQTT-Broker");
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

		private void HandleMessage(MqttApplicationMessage msg)
		{
			var message = JsonConvert.DeserializeObject<Message>(Encoding.UTF8.GetString(msg.Payload));
			messageBusInteractor.Receive(replyTo: message.reply_to, sessionId: message.session_id.FromHex(), message: message.text);
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