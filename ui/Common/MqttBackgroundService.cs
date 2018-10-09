using System;
using System.Net.Mqtt;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace ui.Common
{
    public class Message
    {
        public string session_id  { get; set; }
        public string text { get; set; }
        public string reply_to { get; set; }
    }
    
    public class MqttBackgroundService : BackgroundService 
    {
        private readonly BrokerConfig _brokerConfig;
        private readonly ILogger<MqttBackgroundService> _logger;
        // private IMqttClient _mqttClient;
        private IMqttServer _mqttServer;

        public MqttBackgroundService(IOptions<BrokerConfig> brokerConfig, ILoggerFactory loggerFactory)
        {
            _brokerConfig = brokerConfig.Value;
            _logger = loggerFactory.CreateLogger<MqttBackgroundService>();
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
            var client = await CreateClient();
            _logger.LogInformation("Execute");
            
            
            var sessionState = await Connect(client, _brokerConfig.User);
            
            await client.SubscribeAsync(_brokerConfig.Topic, MqttQualityOfService.ExactlyOnce); //QoS2
            
            
            
            client
                .MessageStream
                .Where(msg => msg.Topic == _brokerConfig.Topic)
                .Subscribe( msg =>
                {
                    LogMessage(msg);
                    Publish(client, "response", "Echo", MqttQualityOfService.ExactlyOnce).Wait(stoppingToken);
                });

/* Not subscripe here, 
            await client.SubscribeAsync("heartbeat", MqttQualityOfService.ExactlyOnce); //QoS2
            client
                .MessageStream
                .Where(msg => msg.Topic == "heartbeat")
                .Subscribe(msg => LogMessage(msg));
   */         

            // sends a initial message on the topic
            await Publish(client, _brokerConfig.Topic, "Hello from C#", MqttQualityOfService.ExactlyOnce);     
         
            
            int qos = 0;
            while (!stoppingToken.IsCancellationRequested)
            {

                // all five second do some lookup for working 
                // ...
                await Publish(client, "heartbeat", $"abc {qos}", (MqttQualityOfService)qos);

                qos = qos + 1;
                if (qos > 2) qos = 0;     
                await Task.Delay(1000, stoppingToken);
            }
            
            //Method to unsubscribe a topic or many topics, which means that the message will no longer
            //be received in the MessageStream anymore
            await client.UnsubscribeAsync(_brokerConfig.Topic);
            
            await Disconnect(client);
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
            _mqttServer.Stop();
            _mqttServer.Dispose();
            return base.StopAsync(cancellationToken);
        }
        
        private void LogMessage(MqttApplicationMessage msg)
        {
            var message = JsonConvert.DeserializeObject<Message>(Encoding.UTF8.GetString(msg.Payload));
            
            _logger.LogInformation($"Message received in topic {msg.Topic}");
            _logger.LogInformation($"{message.session_id} ´{message.text}´");
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
        
        private async Task Publish(IMqttClient client, string topic, string payload, MqttQualityOfService qos)
        {
            _logger.LogInformation($"Publish to topic {topic}");
            var message = new Message()
            {
                session_id = "123435", 
                text = payload
            };
            var json = JsonConvert.SerializeObject(message, Formatting.Indented);
            var msg = new MqttApplicationMessage(topic, Encoding.UTF8.GetBytes(json));
            await client.PublishAsync(msg, qos);
        }
        
    }
}