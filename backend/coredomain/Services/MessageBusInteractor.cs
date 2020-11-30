
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
			LockSystem systemAfter = message.Split("\n")
				.Where(x => !string.IsNullOrWhiteSpace(x))
				.Aggregate(systemBefore, (accu, current) =>
		   {
			   int index = current.IndexOf(',');
			   string head = index == -1 ? current : current.Substring(0, index);
			   if (head.Equals("LE"))
			   {
				   return ListEvents(accu, replyTo, jobId, null);
			   }
			   else if (head.Equals("LD"))
			   {
				   return ListData(accu, replyTo, jobId);
			   }
			   else
			   {
				   return accu.DefineStatement(current);
			   }
		   });

			lockSystem.SetValue(x => systemAfter);

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
				})).Concat(LockSystem.UpdatedEntities(systemBefore, systemAfter).Select(entity =>
				{
					switch (entity)
					{
						case Key key:
							return UpdatedKey(key);
						
						case Lock @lock:
							return UpdatedLock(@lock);
						
						default:
							throw new ArgumentException(
								message: "Missing 'UpdatedXXX' operation for entity",
								paramName: nameof(entity));
					}

				}));

				if (!statements.IsEmpty())
				{
					statements
					   .ToObservable()
					   .Aggregate((accu, current) => accu + "\n" + current)
					   .Select(s => new Statement(replyTo, jobId, s))
					   .Subscribe(this.statementObserver);
				}
			}
		}

		private LockSystem ListEvents(LockSystem lockSystem, string replyTo, int jobId, DateTime? since)
		{
			lockSystem.Events
				.Select(@event => String.Join(',', FormatEvent(@event)))
				.Append("LER,OK")
				.ToObservable()
				.Aggregate((accu, current) => accu + "\n" + current)
				.Select(s => new Statement(replyTo, jobId, s))
				.Subscribe(this.statementObserver);

			return lockSystem;
		}

		private LockSystem ListData(LockSystem lockSystem, string replyTo, int jobId)
		{
			lockSystem.Keys
				.Select(key => String.Join(',', FormatKey(key)))
				.Concat(lockSystem.Locks
					.Select(@lock => String.Join(',', FormatLock(@lock))))
				.Append("LDR,OK")
				.ToObservable()
				.Aggregate((accu, current) => accu + "\n" + current)
				.Select(s => new Statement(replyTo, jobId, s))
				.Subscribe(this.statementObserver);

			return lockSystem;
		}

		private string DefinedKey(Key key) => $"DK,{key.Id},,,,{key.ExtData}";
		private string UpdatedKey(Key key) => $"DK,{key.Id},,,,{(key.ExtData+"\0").ToBase64()}";
		private string DefinedLock(Lock @lock) => $"DL,{@lock.Id},,,,{@lock.ExtData}";
		private string UpdatedLock(Lock @lock) => $"DL,{@lock.Id},,,,{(@lock.ExtData+"\0").ToBase64()}";
		private string DefinedAccessPolicy(LockSystem system, AccessPolicy accessPolicy) =>
			system.QueryKey(accessPolicy.Id) != (Key)null ?
				$"AKR,{accessPolicy.Id},OK" : $"ALR,{accessPolicy.Id},OK";
		private string RemovedKey(Key key) => $"RKR,{key.Id},OK";
		private string RemovedLock(Lock @lock) => $"RDR,{@lock.Id},OK";

		private IEnumerable<string> FormatKey(Key key)
		{
			yield return "DK";
			yield return key.Id;
			yield return key.Name;
			yield return key.Func;
			yield return key.AppId;
			yield return $"{key.ExtData}\0".ToBase64();
		}

		private IEnumerable<string> FormatLock(Lock @lock)
		{
			yield return "DL";
			yield return @lock.Id;
			yield return @lock.Name;
			yield return @lock.Func;
			yield return @lock.AppId;
			yield return $"{@lock.ExtData}\0".ToBase64();
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
	}
}