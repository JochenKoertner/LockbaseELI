using System;
using System.Diagnostics;
using Lockbase.CoreDomain.Enumerations;

namespace Lockbase.CoreDomain.Entities
{

	// Repräsentert ein aufgetretendes 'Event' als Entität
	[DebuggerDisplay("{debugDescription,nq}")]
	public class Event : Entity, IEquatable<Event>
	{

		public Event(string id, DateTime occurredOn, Key key, Lock @lock, EventType eventType) : base(id)
		{
			OccurredOn = occurredOn;
			Key = key;
			Lock = @lock;
			EventType = eventType;
		}

		public DateTime OccurredOn { get; private set; }
		public Key Key { get; private set; }
		public Lock Lock { get; private set; }
		public EventType EventType { get; private set; }

		public bool Equals(Event other) => base.Equals(other);

		private string debugDescription
		=> String.Join(',',
						new[] {"DE", this.Id, this.Key.Id, this.Lock.Id,
								this.OccurredOn.ToShortDateString(), this.EventType.Name});
	}
}