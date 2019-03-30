using System;
using Lockbase.CoreDomain.Entities;

using System.Collections.Immutable;
using System.Collections.Generic;
using System.Linq;
using Lockbase.CoreDomain.ValueObjects;

namespace Lockbase.CoreDomain.Aggregates {

    public class LockSystem {

        private readonly IImmutableDictionary<string, Key> keys;
        private readonly IImmutableDictionary<string, Lock> locks;
        private readonly IImmutableDictionary<string, AccessPolicy> policies;

        private readonly IImmutableSet<PolicyAssignment> assignments;

        public LockSystem():this(
            ImmutableDictionary<string,Key>.Empty,
            ImmutableDictionary<string,Lock>.Empty,
            ImmutableDictionary<string,AccessPolicy>.Empty,
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
        public LockSystem AddKey(Key key) {
            return WithKeys(this.keys.Add(key.Id, key));
        }

        // CL ...
        public LockSystem AddLock(Lock @lock) {
             return WithLocks(this.locks.Add(@lock.Id, @lock));
        }

        // AT
        public LockSystem AddPolicy(AccessPolicy accessPolicy) {
            return WithPolicies(this.policies.Add(accessPolicy.Id, accessPolicy));
        }

        #endregion

        #region Remove Objects 

        public LockSystem RemoveKey(Key key) {
            if (!this.keys.ContainsKey(key.Id)) throw new ArgumentException($"Key '{key.Id}' not found!");
            return WithKeys(this.keys.Remove(key.Id));
        }


        public LockSystem RemoveLock(Lock @lock) {
            if (!this.locks.ContainsKey(@lock.Id)) throw new ArgumentException($"Lock '{@lock.Id}' not found!");
            return WithLocks(this.locks.Remove(@lock.Id));
        }

        #endregion 

        #region Access Programming 

        // AT, ... 
        public LockSystem AddAccessPolicy(AccessPolicy accessPolicy) {
            return WithPolicies(this.policies.Add(accessPolicy.Id, accessPolicy));
        }

        // AK,[ID:KID],[ID:TID],[ID:LID1],[ID:LID2],[ID:LID3],...
        public LockSystem AddAssignment(Key key, AccessPolicy accessPolicy, IEnumerable<Lock> locks) => 
            WithAssignments(this.assignments.Add(
                new PolicyAssignment(accessPolicy, 
                    new Either<LockAssignment,KeyAssignment>(new KeyAssignment(key, locks)))));

        // AL,[ID:LID],[ID:TID],[ID:KID1],[ID:KID2],[ID:KID3],...
         public LockSystem AddAssignment(Lock @lock, AccessPolicy accessPolicy, IEnumerable<Key> keys) =>
            WithAssignments(this.assignments.Add(
                new PolicyAssignment(accessPolicy, 
                    new Either<LockAssignment,KeyAssignment>(new LockAssignment(@lock, keys)))));


        #endregion
        
        public (LockSystem,bool IsOpen) HasAccess(Key key, Lock @lock, DateTime time) {
            if (!this.keys.ContainsKey(key.Id)) throw new ArgumentException($"Key '{key.Id}' not found!");
            if (!this.locks.ContainsKey(@lock.Id)) throw new ArgumentException($"Lock '{@lock.Id}' not found!");

            var definitions = QueryPolicies(@lock, key)
                .SelectMany( policy => policy.TimePeriodDefinitions);

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
                .Where ( assignment => assignment.Match(@lock, key))
                .Select( assignment => assignment.Source);

        #endregion
    }
}
