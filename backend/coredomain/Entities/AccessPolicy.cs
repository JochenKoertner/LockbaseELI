using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Lockbase.CoreDomain.ValueObjects;

namespace Lockbase.CoreDomain.Entities {

	// Repräsentert eine 'Zugriffsrichtlinie' Entität für Schlüsselmedien
	[DebuggerDisplay("{debugDescription,nq}")]

	public class AccessPolicy:Entity, IEquatable<AccessPolicy> {

		public AccessPolicy(string id, NumberOfLockings numberOfLockings, IEnumerable<TimePeriodDefinition> timePeriodDefinitions)
			:base(id) 
		{
			NumberOfLockings = numberOfLockings;
			TimePeriodDefinitions = timePeriodDefinitions
				.Aggregate( 
					ImmutableArray<TimePeriodDefinition>.Empty, (accu, current) => accu.Add(current) );
		}
		
		public NumberOfLockings NumberOfLockings { get; private set;  } 
		public IEnumerable<TimePeriodDefinition> TimePeriodDefinitions { get; private set;  } 

		public bool Equals(AccessPolicy other) => base.Equals(other);

		private string debugDescription => String.Join(',', new[] { "AT", this.Id });


	}
}
