using System.Net.Mqtt;
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
                Port = 1883,
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
            await Task.CompletedTask;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Start");
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stop");
            return base.StopAsync(cancellationToken);
        }
    }
}