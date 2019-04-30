
using Lockbase.CoreDomain.Aggregates;
using Microsoft.Extensions.Logging;

namespace Lockbase.CoreDomain.Services
{

	public class MessageBusInteractor : IMessageBusInteractor
	{

		private readonly ILogger<MessageBusInteractor> logger;
		private readonly AtomicValue<LockSystem> lockSystem;

		public MessageBusInteractor(AtomicValue<LockSystem> lockSystem, ILogger<MessageBusInteractor> logger)
		{
			this.lockSystem = lockSystem;
			this.logger = logger;
		}
		public void Receive(string topic, int sessionId, string message)
		{
			lockSystem.SetValue(x => x.DefineStatement(message));
		}
	}
}
