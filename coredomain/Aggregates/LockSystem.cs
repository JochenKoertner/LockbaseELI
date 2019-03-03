using System;
using Lockbase.CoreDomain.Entities;

using System.Collections.Immutable;
using System.Collections.Generic;

namespace Lockbase.CoreDomain.Aggregates {

    public class LockSystem {

        private readonly IImmutableDictionary<string, Key> keys;
        private readonly IImmutableDictionary<string, Lock> locks;
        private readonly IImmutableDictionary<string, AccessPolicy> policies;

        public LockSystem():this(
            ImmutableDictionary<string,Key>.Empty,
            ImmutableDictionary<string,Lock>.Empty,
            ImmutableDictionary<string,AccessPolicy>.Empty)
        {
        }

        private LockSystem(
            IImmutableDictionary<string, Key> keys, 
            IImmutableDictionary<string, Lock> locks, 
            IImmutableDictionary<string, AccessPolicy> policies)
        {
            this.keys = keys;
            this.locks = locks;
            this.policies = policies;
        }

        #region Create new objects 
        
        // CK ...
        public LockSystem AddKey(Key key) {
            return new LockSystem(this.keys.Add(key.Id, key), this.locks, this.policies);
        }

        // CL ...
        public LockSystem AddLock(Lock @lock) {
             return new LockSystem(this.keys, this.locks.Add(@lock.Id, @lock), this.policies);
        }

        #endregion

        #region Remove Objects 

        public LockSystem RemoveKey(Key key) {
            if (!this.keys.ContainsKey(key.Id)) throw new ArgumentException($"Key '{key.Id}' not found!");
            return new LockSystem(this.keys.Remove(key.Id), this.locks, this.policies);
        }


        public LockSystem RemoveLock(Lock @lock) {
            if (!this.locks.ContainsKey(@lock.Id)) throw new ArgumentException($"Lock '{@lock.Id}' not found!");
            return new LockSystem(this.keys, this.locks.Remove(@lock.Id), this.policies);
        }

        #endregion 

        #region Access Programming 

        // AT, ... 
        public LockSystem AddAccessPolicy(AccessPolicy accessPolicy) {
            return new LockSystem(this.keys, this.locks, this.policies.Add(accessPolicy.Id, accessPolicy));
        }

        // AK,[ID:KID],[ID:TID],[ID:LID1],[ID:LID2],[ID:LID3],...
        public LockSystem AddAssignment(Key key, AccessPolicy accessPolicy, IEnumerable<Lock> locks) {
            return this;
        }

        // AL,[ID:LID],[ID:TID],[ID:KID1],[ID:KID2],[ID:KID3],...
         public LockSystem AddAssignment(Lock @lock, AccessPolicy accessPolicy, IEnumerable<Key> keys) {
            return this;
        }

        #endregion
        
        public bool HasAccess(Key key, Lock @lock, DateTime time) {
            if (!this.keys.ContainsKey(key.Id)) throw new ArgumentException($"Key '{key.Id}' not found!");
            if (!this.locks.ContainsKey(@lock.Id)) throw new ArgumentException($"Lock '{@lock.Id}' not found!");
            return false;
        }

        public IEnumerable<Key> Keys => this.keys.Values;
        public IEnumerable<Lock> Locks => this.locks.Values;
    }
}
