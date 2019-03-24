using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Lockbase.CoreDomain.ValueObjects;

namespace Lockbase.CoreDomain.Entities {

	// Repräsentert eine 'Zugriffsrichtlinie' Entität für Schlüsselmedien
	public class AccessPolicy : IEquatable<AccessPolicy> {

		public AccessPolicy(string id, NumberOfLockings numberOfLockings, IEnumerable<TimePeriodDefinition> timePeriodDefinitions) {

			if (string.IsNullOrWhiteSpace(id)) throw new ArgumentNullException(nameof(id));

			Id = id;
			NumberOfLockings = numberOfLockings;
			TimePeriodDefinitions = timePeriodDefinitions
				.Aggregate( 
					ImmutableArray<TimePeriodDefinition>.Empty, (accu, current) => accu.Add(current) );
		}

		public string Id { get; private set;  } 
		public NumberOfLockings NumberOfLockings { get; private set;  } 
		public IEnumerable<TimePeriodDefinition> TimePeriodDefinitions { get; private set;  } 

		public bool Equals(AccessPolicy other)
        {
            if (other == null)
                return false;

            return (this.Id == other.Id);
        }

		public override bool Equals(object other) {
			if (other == null)
				return false;
			if (other is AccessPolicy)
				return this.Equals((AccessPolicy)other);
			if (other is String) 
				return this.Id.Equals((String)other);
			return false;
		}

		public override int GetHashCode() {
			return this.Id.GetHashCode();
		}
	}
}
