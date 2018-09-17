using ui.Common.EventBus.Events;

namespace ui.Common.EventBus.Abstractions
{
    // https://github.com/dotnet/docs/blob/master/docs/standard/microservices-architecture/multi-container-microservice-net-applications/integration-event-based-microservice-communications.md
    
    // https://github.com/dotnet-architecture/eShopOnContainers/blob/master/src/BuildingBlocks/EventBus/EventBus/Abstractions/IEventBus.cs
    public interface IEventBus
    {
        void Publish(IntegrationEvent @event);

        void Subscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>;
        
        void Unsubscribe<T, TH>()
            where TH : IIntegrationEventHandler<T>
            where T : IntegrationEvent;
    }
}