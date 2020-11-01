
using System;
using System.Collections.Generic;
using System.Linq;
using Lockbase.CoreDomain.Aggregates;
using Lockbase.CoreDomain.Entities;
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
			this.logger.LogInformation($"Receive('{replyTo}', {sessionId.ToString("X8")}, ...)");
			foreach (var line in message.Split("\n").Where(x => !string.IsNullOrWhiteSpace(x)))
			{
				int index = line.IndexOf(',');
				string head = line.Substring(0, index);
				if (head.Equals("LE")) {
					ListEvents(replyTo, sessionId, null);
				} else if (head.Equals("LD")) {
					ListData(replyTo, sessionId);
				} else if (head.Equals("OPEN") || head.Equals("CLOSE")) {
					this.logger.LogInformation(line);
				} else {
					lockSystem.SetValue(x => x.DefineStatement(line));
				}
			}
		}

		private void ListEvents(string topic, int sessionId, DateTime? since)
		{
			this.logger.LogInformation("List Events");
			LockSystem system = this.lockSystem;
			foreach (var @event in system.Events)
				this.statementObserver.OnNext(new Statement(topic, sessionId, String.Join(',', FormatEvent(@event))));

			this.statementObserver.OnNext(new Statement(topic, sessionId,
				$"LER,OK"));
		}

		private IEnumerable<string> FormatEvent(Event @event)
		{
			yield return "EK";
			yield return @event.Lock.Id;
			yield return @event.EventType.Name;
			yield return TimePeriodDefinition.DateTimeToString(@event.OccurredOn);
			yield return string.Empty;
			yield return @event.Key.Id;
		}

		private void ListData(string topic, int sessionId)
		{
			this.logger.LogInformation("List Data");
			this.statementObserver.OnNext(new Statement(topic, sessionId,
				$"LD,blapllop..."));
		}
	}
}