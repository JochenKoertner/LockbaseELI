
using System;
using Lockbase.CoreDomain.Aggregates;
using Lockbase.CoreDomain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Lockbase.CoreDomain.Services
{

	public class MessageBusInteractor : IMessageBusInteractor
	{

		private readonly ILogger<MessageBusInteractor> logger;
		private readonly AtomicValue<LockSystem> lockSystem;

		private readonly IObserver<Statement> statementObserver;

			// statementObserver.OnNext(new Statement(TOPIC_RESPONSE, 4711, 
			// 	$"EK,{@event.Lock.Id},{@event.Key.Id},{@event.IsOpen}"));

		public MessageBusInteractor(AtomicValue<LockSystem> lockSystem, ILogger<MessageBusInteractor> logger, 
			IObserver<Statement> statementObserver)
		{
			this.lockSystem = lockSystem;
			this.logger = logger;
			this.statementObserver = statementObserver;
		}
		public void Receive(string replyTo, int sessionId, string message)
		{
			this.logger.LogInformation($"Receive('{replyTo}', {sessionId}, ...)");
			foreach(var line in message.Split("\n"))
			{
				int index = line.IndexOf(',');
            	string head = line.Substring(0, index);
				if (head.Equals("LE")) {
					ListEvents(replyTo, sessionId);
				} else if (head.Equals("LD")) {
					this.logger.LogInformation("List Data");
				} else if (head.Equals("OPEN") || head.Equals("CLOSE")) {
					this.logger.LogInformation(line);
				} else {
					lockSystem.SetValue(x => x.DefineStatement(line));
				}
			}
		}

		private void ListEvents(string topic, int sessionId) {
			this.logger.LogInformation("List Events");
			this.statementObserver.OnNext(new Statement(topic, sessionId, 
				$"EK,sddsds"));
		}

		private void ListData() {

		}
	}
}