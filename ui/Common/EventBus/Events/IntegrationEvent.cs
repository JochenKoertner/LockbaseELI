using System;

namespace ui.Common.EventBus.Events
{
    public abstract class IntegrationEvent
    {
        public Guid Id  { get; } = Guid.NewGuid();
        public DateTime CreationDate { get; } = DateTime.UtcNow;
    }
}