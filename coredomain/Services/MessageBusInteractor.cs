
using Lockbase.CoreDomain.Aggregates;
using Microsoft.Extensions.Logging;

namespace Lockbase.CoreDomain.Services {

    public class MessageBusInteractor : IMessageBusInteractor {

        private readonly ILogger<MessageBusInteractor> logger;
        private readonly AtomicValue<LockSystem> lockSystem;

        public MessageBusInteractor(AtomicValue<LockSystem> lockSystem, ILogger<MessageBusInteractor> logger)
		{
			this.lockSystem = lockSystem;
			this.logger = logger;	
		}
        public void Receive(string topic, string session_id, string message) {
            logger.LogInformation($"Message received in topic {topic}");
            logger.LogInformation($"{session_id} ´{message}´");
        }

        public void Publish(string topic, string session_id, string message) {}
    }
}
