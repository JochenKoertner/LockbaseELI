using System;
using System.Collections.Immutable;
using System.Collections.Generic;
using System.Linq;
using Lockbase.CoreDomain.Entities;
using Lockbase.CoreDomain.ValueObjects;
using Lockbase.CoreDomain.Extensions;
using Lockbase.CoreDomain.Services;
using Lockbase.CoreDomain.Enumerations;
using System.Diagnostics.CodeAnalysis;

namespace Lockbase.CoreDomain.Aggregates
{
	public class LockSystem : IEquatable<LockSystem>
	{
		public static LockSystem Create(Id id) => new LockSystem(id);

		private readonly Id id;

		private readonly IImmutableDictionary<string, Key> keys;
		private readonly IImmutableDictionary<string, Lock> locks;
		private readonly IImmutableDictionary<string, AccessPolicy> policies;

		private readonly IImmutableSet<PolicyAssignment> assignments;
		private readonly IImmutableList<Event> events;

		public IEnumerable<Event> Events => events;

		private LockSystem(Id id) : this(
			id,
			ImmutableDictionary<string, Key>.Empty,
			ImmutableDictionary<string, Lock>.Empty,
			ImmutableDictionary<string, AccessPolicy>.Empty,
			ImmutableHashSet<PolicyAssignment>.Empty,
			ImmutableList<Event>.Empty)
		{
		}

		private LockSystem(
			Id id,
			IImmutableDictionary<string, Key> keys,
			IImmutableDictionary<string, Lock> locks,
			IImmutableDictionary<string, AccessPolicy> policies,
			IImmutableSet<PolicyAssignment> assignments,
			IImmutableList<Event> events)
		{
			this.id = id;
			this.keys = keys;
			this.locks = locks;
			this.policies = policies;
			this.assignments = assignments;
			this.events = events;
		}

		private LockSystem WithKeys(IImmutableDictionary<string, Key> keys)
		{
			return new LockSystem(this.id, keys: keys,
				locks: this.locks, policies: this.policies, assignments: this.assignments, events: this.events);
		}

		private LockSystem WithLocks(IImmutableDictionary<string, Lock> locks)
		{
			return new LockSystem(this.id, locks: locks,
				keys: this.keys, policies: this.policies, assignments: this.assignments, events: this.events);
		}

		private LockSystem WithPolicies(IImmutableDictionary<string, AccessPolicy> policies)
		{
			return new LockSystem(this.id, policies: policies,
				keys: this.keys, locks: this.locks, assignments: this.assignments, events: this.events);
		}

		private LockSystem WithAssignments(IImmutableSet<PolicyAssignment> assignments)
		{
			return new LockSystem(this.id, assignments: assignments,
				keys: this.keys, locks: this.locks, policies: this.policies, events: this.events);
		}
		private LockSystem WithEvents(IImmutableList<Event> events)
		{
			return new LockSystem(this.id, events: events,
				keys: this.keys, locks: this.locks, assignments: assignments, policies: this.policies);
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
			var name = properties.ElementAtOrDefault(1);
			var appId = properties.ElementAtOrDefault(2);
			var extData = properties.ElementAtOrDefault(3)
				.FromBase64()
				.RemoveTrailingZero();

			return (this.locks.TryGetValue(id, out var @lock)) ?
				RemoveLock(@lock)
				.AddLock(new Lock(id: @lock.Id, name: @lock.Name, appId: @lock.AppId, extData: extData))
				:
				AddLock(new Lock(id: id, name: name, appId: appId, extData: extData));
		}

		public LockSystem DefineKey(string statement)
		{
			var properties = statement.Split(',');
			var id = properties.ElementAt(0);
			var name = properties.ElementAtOrDefault(1);
			var appId = properties.ElementAtOrDefault(2);
			var extData = properties.ElementAtOrDefault(3)
				.FromBase64()
				.RemoveTrailingZero();

			return (this.keys.TryGetValue(id, out var key)) ?
				RemoveKey(key)
				.AddKey(new Key(id: key.Id, name: key.Name, appId: key.AppId, extData: extData))
				:
				AddKey(new Key(id: id, name: name, appId: appId, extData: extData));
		}

		public LockSystem CreateKey(string statement)
		{
			var properties = statement.Split(',');
			var appId = properties.ElementAtOrDefault(0);
			var name = properties.ElementAtOrDefault(1);
			return AddKey(new Key(this.id.NewId(TableIds.Key, this.Keys.Count() + 1), name: name, appId: appId));
		}

		public LockSystem CreateLock(string statement)
		{
			var properties = statement.Split(',');
			var appId = properties.ElementAtOrDefault(0);
			var name = properties.ElementAtOrDefault(1);
			return AddLock(new Lock(this.id.NewId(TableIds.Lock, this.Locks.Count() + 1), name: name, appId: appId));
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
			else if (head.Equals("CK"))
				return CreateKey(tail);
			else if (head.Equals("RK"))
				return RemoveKey(tail);
			else if (head.Equals("DL"))
				return DefineLock(tail);
			else if (head.Equals("CL"))
				return CreateLock(tail);
			else if (head.Equals("RL"))
				return RemoveLock(tail);
			else if (head.Equals("AT"))
				return DefinePolicy(tail);
			else if (head.Equals("AL"))
				return DefineAssignmentLock(tail);
			else if (head.Equals("AK"))
				return DefineAssignmentKey(tail);
			else if (head.Equals("OPEN") || head.Equals("CLOSE"))
				return this;
			else if (head.Equals("LE"))
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

		private LockSystem RemoveKey(string statement) 
		{
			var properties = statement.Split(',');
			var id = properties.ElementAt(0);
			if (!this.keys.ContainsKey(id)) throw new ArgumentException($"Key '{id}' not found!");
			return WithKeys(this.keys.Remove(id));
		}


		public LockSystem RemoveLock(Lock @lock)
		{
			if (!this.locks.ContainsKey(@lock.Id)) throw new ArgumentException($"Lock '{@lock.Id}' not found!");
			return WithLocks(this.locks.Remove(@lock.Id));
		}

		private LockSystem RemoveLock(string statement) 
		{
			var properties = statement.Split(',');
			var id = properties.ElementAt(0);
			if (!this.locks.ContainsKey(id)) throw new ArgumentException($"Lock '{id}' not found!");
			return WithLocks(this.locks.Remove(id));
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

		#region Access 

		public LockSystem AddEvent(Event @event) =>
		   WithEvents(this.events.Add(@event));

		public EventType HasAccess(Key key, Lock @lock, DateTime dateTime)
		{
			if (!this.keys.ContainsKey(key.Id)) throw new ArgumentException($"Key '{key.Id}' not found!");
			if (!this.locks.ContainsKey(@lock.Id)) throw new ArgumentException($"Lock '{@lock.Id}' not found!");

			var definitions = QueryPolicies(@lock, key)
				.SelectMany(policy => policy.TimePeriodDefinitions);

			return CheckAccess.Check(definitions, dateTime)
				?
				EventType.Authorized_Access : EventType.Unauthorized_Access;
		}

		#endregion

		public IEnumerable<Key> Keys => this.keys.Values;
		public IEnumerable<Lock> Locks => this.locks.Values;
		public IEnumerable<AccessPolicy> Policies => this.policies.Values;
		public IEnumerable<PolicyAssignment> Assignments => this.assignments;


		#region Query

		public Key QueryKey(string id) => this.keys.GetValueOrDefault(id);
		public Lock QueryLock(string id) => this.locks.GetValueOrDefault(id);
		public AccessPolicy QueryPolicy(string id) => this.policies.GetValueOrDefault(id);

		public IEnumerable<AccessPolicy> QueryPolicies(Lock @lock, Key key) =>
			this.assignments
				.Where(assignment => assignment.Match(@lock, key))
				.Select(assignment => assignment.Source);


		#endregion

		public bool Equals([AllowNull] LockSystem other) 
		{
			if (other == null)
				return false;

			return 
				keys.Equals(other.keys) &&
				locks.Equals(other.locks) &&
				policies.Equals(other.policies) &&
				assignments.Equals(other.assignments);
		}

		public static IEnumerable<Entity> CreatedEntities(LockSystem preceding, LockSystem present)
		{
			var newKeys = present.Keys.Except(preceding.Keys).Cast<Entity>();
			var newLocks = present.Locks.Except(preceding.Locks).Cast<Entity>();
			var newPolicies = present.Policies.Except(preceding.Policies).Cast<Entity>();
			return newKeys.Concat(newLocks).Concat(newPolicies).ToList();
		}

		public static IEnumerable<Entity> UpdatedEntities(LockSystem preceding, LockSystem present)
		{
			IEqualityComparer<Key> keyComparer = new KeyComparer();
			var updatedKeys = present.Keys.Intersect(preceding.Keys).Except(preceding.Keys.Intersect(present.Keys), keyComparer).Cast<Entity>();
			IEqualityComparer<Lock> lockComparer = new LockComparer();
			var updatedLocks = present.Locks.Intersect(preceding.Locks).Except(preceding.Locks.Intersect(present.Locks), lockComparer).Cast<Entity>();

			//var newPolicies = present.Policies.Except(preceding.Policies).Cast<Entity>();
			return updatedKeys.Concat(updatedLocks).ToList();
		}

		public static IEnumerable<Entity> RemovedEntities(LockSystem preceding, LockSystem present)
		{
			var removedKeys = preceding.Keys.Except(present.Keys).Cast<Entity>();
			var removedLocks = preceding.Locks.Except(present.Locks).Cast<Entity>();
			return removedKeys.Concat(removedLocks).ToList();
		}
	}
}