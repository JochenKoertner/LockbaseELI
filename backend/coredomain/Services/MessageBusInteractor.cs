
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Lockbase.CoreDomain.Aggregates;
using Lockbase.CoreDomain.Entities;
using Lockbase.CoreDomain.Extensions;
using Lockbase.CoreDomain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Lockbase.CoreDomain.Services
{
	public class Message
	{
		public string text { get; set; }

		public int correlationId { get; set; }

		public string replyTo { get; set; }
	}

	public class MessageBusInteractor
	{

		private readonly ILogger<MessageBusInteractor> logger;
		private readonly AtomicValue<LockSystem> lockSystem;

		private readonly IObserver<Statement> statementObserver;

		// statementObserver.OnNext(new Statement(TOPIC_RESPONSE, 4711, 
		// 	$"EK,{@event.Lock.Id},{@event.Key.Id},{@event.IsOpen}"));

		public MessageBusInteractor(AtomicValue<LockSystem> lockSystem, ILoggerFactory loggerFactory,
			IObserver<Statement> statementObserver, IObservable<Message> messageObservable)
		{
			this.lockSystem = lockSystem;
			this.logger = loggerFactory.CreateLogger<MessageBusInteractor>();

			this.statementObserver = statementObserver;

			messageObservable.Subscribe(msg => Receive(msg.replyTo, msg.correlationId, msg.text));
		}
		private void Receive(string replyTo, int jobId, string message)
		{
			this.logger.LogInformation($"Receive('{replyTo}', {jobId.ToHex()}, '{message.Shorten()}')");

			LockSystem systemBefore = lockSystem;
			foreach (var line in message.Split("\n").Where(x => !string.IsNullOrWhiteSpace(x)))
			{
				int index = line.IndexOf(',');
				string head = index == -1 ? line : line.Substring(0, index);
				if (head.Equals("LE"))
				{
					ListEvents(replyTo, jobId, null);
				}
				else if (head.Equals("LD"))
				{
					ListData(replyTo, jobId);
				}
				else
				{
					lockSystem.SetValue(x => x.DefineStatement(line));
				}
			}
			LockSystem systemAfter = lockSystem;

			if (systemBefore != systemAfter)
			{
				var statements = LockSystem.CreatedEntities(systemBefore, systemAfter).Select(entity =>
					{
						switch (entity)
						{
							case Key key:
								return DefinedKey(key);

							case Lock @lock:
								return DefinedLock(@lock);

							case AccessPolicy accessPolicy:
								return DefinedAccessPolicy(systemAfter, accessPolicy);

							 default:
								 throw new ArgumentException(
									 message: "Missing 'DefinedXXX' operation for entity",
									 paramName: nameof(entity));
						 }
					 }).Concat(LockSystem.RemovedEntities(systemBefore, systemAfter).Select(entity =>
				  {
					switch (entity)
					{
						case Key key:
							return RemovedKey(key);

						case Lock @lock:
							return RemovedLock(@lock);

						default:
							throw new ArgumentException(
								message: "Missing 'RemovedXXX' operation for entity",
								paramName: nameof(entity));
					  }
				  }));

				statements
					.ToObservable()
					.Select(s => new Statement(replyTo, jobId, s))
					.Subscribe(this.statementObserver);
			}
		}

		

		private void ListEvents(string topic, int jobId, DateTime? since)
		{
			LockSystem system = this.lockSystem;
			foreach (var @event in system.Events)
				this.statementObserver.OnNext(new Statement(topic, jobId, String.Join(',', FormatEvent(@event))));

			this.statementObserver.OnNext(new Statement(topic, jobId, $"LER,OK"));
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
			this.statementObserver.OnNext(new Statement(topic, sessionId, "LDR,OK"));
		}

		private string DefinedKey(Key key) => $"DK,{key.Id},,,,{key.ExtData}";
		private string DefinedLock(Lock @lock) => $"DL,{@lock.Id},,,,{@lock.ExtData}";
		private string DefinedAccessPolicy(LockSystem system, AccessPolicy accessPolicy) => 
			system.QueryKey(accessPolicy.Id) != (Key)null ? 
				$"AKR,{accessPolicy.Id},OK" : $"ALR,{accessPolicy.Id},OK";
		private string RemovedKey(Key key) => $"RKR,{key.Id},OK";
		private string RemovedLock(Lock @lock) => $"RDR,{@lock.Id},OK";

	}
}