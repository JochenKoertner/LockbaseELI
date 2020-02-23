using System; 

namespace Lockbase.CoreDomain.Entities {

	// Repräsentert ein aufgetretendes 'Event' als Entität
	public class Event: Entity, IEquatable<Event> {

		public Event(string id, DateTime occurredOn, Key key, Lock @lock, bool isOpen):base(id) {
			OccurredOn = occurredOn;
			Key = key;
			Lock = @lock;
			IsOpen = isOpen;	
		}

		public DateTime OccurredOn { get; private set;  } 
		public Key Key { get; private set;  } 
		public Lock Lock { get; private set;  } 
		public bool IsOpen { get; private set;  } 

		public bool Equals(Event other) => base.Equals(other);
	}
}
