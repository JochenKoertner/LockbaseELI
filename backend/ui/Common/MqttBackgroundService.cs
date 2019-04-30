using System;
using System.Net.Mqtt;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
		private IMqttClient _mqttClient;
		private IMqttServer _mqttServer;
		private IDisposable _observerStatement;
		private IDisposable _observerChannel;
		private IDisposable _observerResponse;

		private readonly IMessageBusInteractor messageBusInteractor;
		private readonly Subject<Statement> statementSubject;

		public MqttBackgroundService(
			IOptions<BrokerConfig> brokerConfig,
			ILoggerFactory loggerFactory,
			IMessageBusInteractor messageBusInteractor,
			Subject<Statement> statementSubject)
		{
			_brokerConfig = brokerConfig.Value;
			_logger = loggerFactory.CreateLogger<MqttBackgroundService>();
			this.messageBusInteractor = messageBusInteractor;
			this.statementSubject = statementSubject;
		}

		private async Task<IMqttClient> CreateClient()
		{
			_logger.LogInformation("CreateClient");
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
			_mqttClient = await CreateClient();
			_logger.LogInformation("Execute");


			var sessionState = await Connect(_mqttClient, _brokerConfig.User);

			await _mqttClient.SubscribeAsync(_brokerConfig.Topic, MqttQualityOfService.ExactlyOnce); //QoS2

			_observerStatement = this.statementSubject.Subscribe(
				async statement =>
					await Publish(_mqttClient, statement.Topic, statement.SessionId, statement.Head,
					 statement.Topic.Equals(TOPIC_HEARTBEAT) ? MqttQualityOfService.AtMostOnce : MqttQualityOfService.ExactlyOnce)
			);

			_observerChannel = _mqttClient
				.MessageStream
				.Where(msg => msg.Topic == _brokerConfig.Topic)
				.Subscribe(msg => HandleMessage(msg));

			_observerResponse = _mqttClient
				.MessageStream
				.Where(msg => msg.Topic == TOPIC_RESPONSE)
				.Subscribe(msg => _logger.LogInformation($"Publish to topic {msg.Topic}"));

			// sends a initial message on the topic
			this.statementSubject.OnNext(new Statement(_brokerConfig.Topic, 4711,"DK,000000hqvs1lo,,,MTAzLTEsIEZlbmRlciwgS2xhdXMA"));
			while (!stoppingToken.IsCancellationRequested)
			{

				// all five second do some lookup for working 
				this.statementSubject.OnNext(new Statement(TOPIC_HEARTBEAT, 4711, $"HC,{DateTime.UtcNow.ToShortTimeString()}"));
				await Task.Delay(5000, stoppingToken);
			}

			//Method to unsubscribe a topic or many topics, which means that the message will no longer
			//be received in the MessageStream anymore
			await _mqttClient.UnsubscribeAsync(_brokerConfig.Topic);

			await Disconnect(_mqttClient);
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
			_observerStatement?.Dispose();
			_observerChannel?.Dispose();
			_observerResponse?.Dispose();
			_mqttClient?.Dispose();

			_mqttServer.Stop();
			_mqttServer.Dispose();

			return base.StopAsync(cancellationToken);
		}

		private void HandleMessage(MqttApplicationMessage msg)
		{
			var message = JsonConvert.DeserializeObject<Message>(Encoding.UTF8.GetString(msg.Payload));
			messageBusInteractor.Receive(replyTo: message.reply_to, session_id: message.session_id, message: message.text);
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

		private async Task Publish(IMqttClient client, string topic, int sessionId, string payload, MqttQualityOfService qos)
		{
			var message = new Message()
			{
				session_id = sessionId.ToString(),
				text = payload
			};
			var json = JsonConvert.SerializeObject(message, Formatting.Indented);
			var msg = new MqttApplicationMessage(topic, Encoding.UTF8.GetBytes(json));
			await client.PublishAsync(msg, qos);
		}
	}
}