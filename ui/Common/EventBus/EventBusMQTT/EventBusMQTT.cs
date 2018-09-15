using System;
using System.Net.Mqtt;
using Microsoft.Extensions.Logging;
using ui.Common.EventBus.Abstractions;
using ui.Common.EventBus.Events;

namespace ui.Common.EventBus.EventBusMQTT
{
    public class EventBusMQTT : IEventBus, IDisposable
    {
        private readonly ILogger<EventBusMQTT> _logger;
        private readonly IEventBusSubscriptionsManager _subsManager;

        public EventBusMQTT(IMqttClient mqttClient, ILogger<EventBusMQTT> logger,
            IEventBusSubscriptionsManager subsManager)
        {
            _logger = logger;
            _subsManager = subsManager;
        }

        public void Publish(IntegrationEvent @event)
        {
            _logger.LogInformation("Publish");
        }
        
        public void Subscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            _logger.LogInformation("Subscripe");
            var eventName = _subsManager.GetEventKey<T>();
            // DoInternalSubscription(eventName);
            _subsManager.AddSubscription<T, TH>();
        }

        public void Unsubscribe<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
           /* if (_consumerChannel != null)
            {
                _consumerChannel.Dispose();
            }
*/
            _subsManager.Clear();
        }


    }
}