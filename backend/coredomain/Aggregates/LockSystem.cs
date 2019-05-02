using System;
using Lockbase.CoreDomain.Entities;

using System.Collections.Immutable;
using System.Collections.Generic;
using System.Linq;
using Lockbase.CoreDomain.ValueObjects;

namespace Lockbase.CoreDomain.Aggregates
{

	public class LockSystem
	{

		static private Lazy<LockSystem> empty = new Lazy<LockSystem>(() => new LockSystem(), true);

		public static LockSystem Empty => empty.Value;

		private readonly IImmutableDictionary<string, Key> keys;
		private readonly IImmutableDictionary<string, Lock> locks;
		private readonly IImmutableDictionary<string, AccessPolicy> policies;

		private readonly IImmutableSet<PolicyAssignment> assignments;

		public LockSystem() : this(
			ImmutableDictionary<string, Key>.Empty,
			ImmutableDictionary<string, Lock>.Empty,
			ImmutableDictionary<string, AccessPolicy>.Empty,
			ImmutableHashSet<PolicyAssignment>.Empty)
		{
		}

		private LockSystem(
			IImmutableDictionary<string, Key> keys,
			IImmutableDictionary<string, Lock> locks,
			IImmutableDictionary<string, AccessPolicy> policies,
			IImmutableSet<PolicyAssignment> assignments)
		{
			this.keys = keys;
			this.locks = locks;
			this.policies = policies;
			this.assignments = assignments;
		}

		private LockSystem WithKeys(IImmutableDictionary<string, Key> keys)
		{
			return new LockSystem(keys: keys,
				locks: this.locks, policies: this.policies, assignments: this.assignments);
		}

		private LockSystem WithLocks(IImmutableDictionary<string, Lock> locks)
		{
			return new LockSystem(locks: locks,
				keys: this.keys, policies: this.policies, assignments: this.assignments);
		}

		private LockSystem WithPolicies(IImmutableDictionary<string, AccessPolicy> policies)
		{
			return new LockSystem(policies: policies,
				keys: this.keys, locks: this.locks, assignments: this.assignments);
		}

		private LockSystem WithAssignments(IImmutableSet<PolicyAssignment> assignments)
		{
			return new LockSystem(assignments: assignments,
				keys: this.keys, locks: this.locks, policies: this.policies);
		}


		#region Create new objects 

		// CK ...
		public LockSystem AddKey(Key key)
		{
			return WithKeys(this.keys.Add(key.Id, key));
		}

		// CL ...
		public LockSystem AddLock(Lock @lock)
		{
			return WithLocks(this.locks.Add(@lock.Id, @lock));
		}

		// AT
		public LockSystem AddPolicy(AccessPolicy accessPolicy)
		{
			return WithPolicies(this.policies.Add(accessPolicy.Id, accessPolicy));
		}

		public LockSystem DefineLock(string statement)
		{
			var properties = statement.Split(',');
			var id = properties.ElementAt(0);
			var appId = properties.ElementAtOrDefault(1);
			var name = properties.ElementAtOrDefault(2);
			var extData = properties.ElementAtOrDefault(3)
				.FromBase64()
				.RemoveTrailingZero();
			return AddLock(new Lock(id: id, name: name, appId: appId, extData: extData));
		}

		public LockSystem DefineKey(string statement)
		{
			var properties = statement.Split(',');
			var id = properties.ElementAt(0);
			var appId = properties.ElementAtOrDefault(1);
			var name = properties.ElementAtOrDefault(2);
			var extData = properties.ElementAtOrDefault(3)
				.FromBase64()
				.RemoveTrailingZero();
			return AddKey(new Key(id: id, name: name, appId: appId, extData: extData));
		}

		public LockSystem DefinePolicy(string statement)
		{
			var properties = statement.Split(',');
			var id = properties.ElementAt(0);
			var numberOfLockings = (NumberOfLockings)properties.ElementAt(1);
			var timePeriodDefinitions = properties.Skip(2).Select(definition => (TimePeriodDefinition)definition);

			return AddPolicy(new AccessPolicy(id: id,
				numberOfLockings: numberOfLockings,
				timePeriodDefinitions: timePeriodDefinitions));
		}

		public LockSystem DefineAssignmentKey(string statement)
		{
			var properties = statement.Split(',');
			var key = QueryKey(properties.ElementAt(0));
			var policy = QueryPolicy(properties.ElementAt(1));
			var locks = properties.Skip(2).Select(id => QueryLock(id));
			return AddAssignment(key, policy, locks);
		}

		public LockSystem DefineAssignmentLock(string statement)
		{
			var properties = statement.Split(',');
			var @lock = QueryLock(properties.ElementAt(0));
			var policy = QueryPolicy(properties.ElementAt(1));
			var keys = properties.Skip(2).Select(id => QueryKey(id));
			return AddAssignment(@lock, policy, keys);
		}

		public LockSystem DefineStatement(string statement)
		{
			int index = statement.IndexOf(',');
			string head = statement.Substring(0, index);
			string tail = statement.Substring(index + 1);

			if (head.Equals("DK"))
				return DefineKey(tail);
			else if (head.Equals("DL"))
				return DefineLock(tail);
			else if (head.Equals("AT"))
				return DefinePolicy(tail);
			else if (head.Equals("AL"))
				return DefineAssignmentLock(tail);
			else if (head.Equals("AK"))
				return DefineAssignmentKey(tail);
			else if (head.Equals("OPEN") || head.Equals("CLOSE"))
				return this;

			throw new NotImplementedException(head);
		}

		#endregion

		#region Remove Objects 

		public LockSystem RemoveKey(Key key)
		{
			if (!this.keys.ContainsKey(key.Id)) throw new ArgumentException($"Key '{key.Id}' not found!");
			return WithKeys(this.keys.Remove(key.Id));
		}


		public LockSystem RemoveLock(Lock @lock)
		{
			if (!this.locks.ContainsKey(@lock.Id)) throw new ArgumentException($"Lock '{@lock.Id}' not found!");
			return WithLocks(this.locks.Remove(@lock.Id));
		}

		#endregion 

		#region Access Programming 

		// AT, ... 
		public LockSystem AddAccessPolicy(AccessPolicy accessPolicy)
		{
			return WithPolicies(this.policies.Add(accessPolicy.Id, accessPolicy));
		}

		// AK,[ID:KID],[ID:TID],[ID:LID1],[ID:LID2],[ID:LID3],...
		public LockSystem AddAssignment(Key key, AccessPolicy accessPolicy, IEnumerable<Lock> locks) =>
			WithAssignments(this.assignments.Add(
				new PolicyAssignment(accessPolicy,
					new Either<LockAssignment, KeyAssignment>(new KeyAssignment(key, locks)))));

		// AL,[ID:LID],[ID:TID],[ID:KID1],[ID:KID2],[ID:KID3],...
		public LockSystem AddAssignment(Lock @lock, AccessPolicy accessPolicy, IEnumerable<Key> keys) =>
		   WithAssignments(this.assignments.Add(
			   new PolicyAssignment(accessPolicy,
				   new Either<LockAssignment, KeyAssignment>(new LockAssignment(@lock, keys)))));


		#endregion

		public (LockSystem, bool IsOpen) HasAccess(Key key, Lock @lock, DateTime time)
		{
			if (!this.keys.ContainsKey(key.Id)) throw new ArgumentException($"Key '{key.Id}' not found!");
			if (!this.locks.ContainsKey(@lock.Id)) throw new ArgumentException($"Lock '{@lock.Id}' not found!");

			var definitions = QueryPolicies(@lock, key)
				.SelectMany(policy => policy.TimePeriodDefinitions);

			var newSystem = this;
			return (newSystem, IsOpen: CheckAccess.Check(definitions, time));
		}

		public IEnumerable<Key> Keys => this.keys.Values;
		public IEnumerable<Lock> Locks => this.locks.Values;
		public IEnumerable<AccessPolicy> Policies => this.policies.Values;

		#region Query

		public Key QueryKey(string id) => this.keys.GetValueOrDefault(id);
		public Lock QueryLock(string id) => this.locks.GetValueOrDefault(id);
		public AccessPolicy QueryPolicy(string id) => this.policies.GetValueOrDefault(id);

		public IEnumerable<AccessPolicy> QueryPolicies(Lock @lock, Key key) =>
			this.assignments
				.Where(assignment => assignment.Match(@lock, key))
				.Select(assignment => assignment.Source);

		#endregion
	}
}
