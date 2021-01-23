
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Lockbase.CoreDomain.Entities;

namespace Lockbase.CoreDomain.ValueObjects
{
	public class Assignment<TMaster, TDetail> : IEquatable<Assignment<TMaster, TDetail>>
		where TMaster : IEquatable<TMaster>
		where TDetail : IEquatable<TDetail>
	{
		public TMaster Master { get; init; }

		public ISet<TDetail> Details { get; init; }

		public Assignment(TMaster master, IEnumerable<TDetail> details)
		{
			Master = master;
			Details = details.ToImmutableHashSet();
		}

		public bool Equals(Assignment<TMaster, TDetail> other)
		{
			if (other == null)
				return false;

			if (this.Master.Equals(other.Master) && this.Details.SetEquals(other.Details))
				return true;
			else
				return false;
		}

		public override int GetHashCode()
		{
			return this.Master.GetHashCode() * 13 + this.Details.GetHashCode();
		}
	}
	public class LockAssignment : Assignment<Lock, Key>, IEquatable<LockAssignment>
	{

		public LockAssignment(Lock @lock, IEnumerable<Key> keys) : base(@lock, keys)
		{
		}

		public bool Equals(LockAssignment other)
		{
			return base.Equals(other);
		}
	}

	public class KeyAssignment : Assignment<Key, Lock>, IEquatable<KeyAssignment>
	{

		public KeyAssignment(Key key, IEnumerable<Lock> locks) : base(key, locks)
		{
		}

		public bool Equals(KeyAssignment other)
		{
			return base.Equals(other);
		}
	}

	public class PolicyAssignment : Entity, IEquatable<PolicyAssignment>
	{

		public AccessPolicy Source { get; init; }
		public Either<LockAssignment, KeyAssignment> Target { get; init; }

		public PolicyAssignment(AccessPolicy source, Either<LockAssignment, KeyAssignment> target) : base(source.Id)
		{
			Source = source;
			Target = target;
		}

		public bool Equals(PolicyAssignment other)
		{
			return (this.Source.Equals(other.Source) && this.Target.Equals(other.Target));
		}

		public bool Match(Lock @lock, Key key) => Target.Match(
				lockAssignment => lockAssignment.Master == @lock && lockAssignment.Details.Contains(key),
				keyAssignment => keyAssignment.Master == key && keyAssignment.Details.Contains(@lock));
	}
}