using System;
using System.Net.Mqtt;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ui.Common
{
    public class MqttBackgroundService : BackgroundService 
    {
        private readonly BrokerConfig _brokerConfig;
        private readonly ILogger<MqttBackgroundService> _logger;
        private IMqttClient _mqttClient;
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
            _mqttClient = await CreateClient();
            _logger.LogInformation("Execute");
            
            
            var sessionState = await Connect(_mqttClient, _brokerConfig.User);
            
            await _mqttClient.SubscribeAsync(_brokerConfig.Topic, MqttQualityOfService.ExactlyOnce); //QoS2
            
            _mqttClient
                .MessageStream
                .Subscribe(msg => Console.WriteLine($"Message received in topic {msg.Topic}"));
  	
            _mqttClient
                .MessageStream
                .Where(msg => msg.Topic == _brokerConfig.Topic)
                .Subscribe(LogMessage);

            // sends a initial message on the topic
            await Publish(_mqttClient, _brokerConfig.Topic, "Hello from C#", MqttQualityOfService.ExactlyOnce);     
         
            
            while (!stoppingToken.IsCancellationRequested)
            {

                // This eShopOnContainers method is querying a database table 
                // and publishing events into the Event Bus (RabbitMS / ServiceBus)
                // CheckConfirmedGracePeriodOrders();

                await Task.Delay(5000, stoppingToken);
            }
            
            _logger.LogDebug($"GracePeriod background task is stopping.");
            
            
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
            _mqttServer.Stop();
            _mqttServer.Dispose();
            return base.StopAsync(cancellationToken);
        }
        
        private void LogMessage(MqttApplicationMessage msg)
        {
            _logger.LogInformation($"Message received in topic {msg.Topic}");
            _logger.LogInformation($"´{Encoding.UTF8.GetString(msg.Payload)}´");
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
            var message = new MqttApplicationMessage(topic, Encoding.UTF8.GetBytes(payload));
            await client.PublishAsync(message, qos);
        }
    }
}