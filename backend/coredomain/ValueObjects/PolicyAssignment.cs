
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Lockbase.CoreDomain.Entities;

namespace Lockbase.CoreDomain.ValueObjects
{


	public class Assignment<TMaster,TDetail> : IEquatable<Assignment<TMaster,TDetail>> 
		where TMaster : IEquatable<TMaster>
		where TDetail : IEquatable<TDetail> 
	{
		public readonly TMaster Master;

		public readonly ISet<TDetail> Details;

		public Assignment(TMaster master, IEnumerable<TDetail> details)
		{
			Master = master;
			Details = details.ToImmutableHashSet();
		}

		public bool Equals(Assignment<TMaster,TDetail> other)
		{
			if (other == null)
				return false;

			if (this.Master.Equals(other.Master) && this.Details.SetEquals(other.Details))
				return true;
			 else
				 return false;
		}
	}
	public class LockAssignment : Assignment<Lock,Key> {
		
		public LockAssignment(Lock @lock, IEnumerable<Key> keys):base(@lock, keys) 
		{
		}
	}
	
	public class KeyAssignment : Assignment<Key,Lock> {
		
		public KeyAssignment(Key key, IEnumerable<Lock> locks):base(key, locks) 
		{
		}
	}
	

	public readonly struct PolicyAssignment : IEquatable<PolicyAssignment> {
			
		public readonly AccessPolicy Source;
		public readonly Either<LockAssignment,KeyAssignment> Target;

		public PolicyAssignment(AccessPolicy source, Either<LockAssignment,KeyAssignment> target) 
			=> (Source, Target) = (source,target);

		public bool Equals(PolicyAssignment other)
		{
			return (this.Source.Equals(other.Source) && this.Target.Equals(other.Target));
		}

		public bool Match(Lock @lock, Key key) => Target.Match( 
				lockAssignment => lockAssignment.Master == @lock && lockAssignment.Details.Contains(key), 
				keyAssignment => keyAssignment.Master == key && keyAssignment.Details.Contains(@lock) );
	}
}