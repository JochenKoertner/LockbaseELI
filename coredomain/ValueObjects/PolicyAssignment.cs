
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Lockbase.CoreDomain.Entities;

namespace Lockbase.CoreDomain.ValueObjects  {


    public class Assignment<TMaster,TDetail> : IEquatable<Assignment<TMaster,TDetail>> 
        where TMaster : IEquatable<TMaster>
        where TDetail : IEquatable<TDetail> 
    {
        public TMaster Master { get; private set; }

        public ISet<TDetail> Details { get; private set; }

        public Assignment(TMaster master, IEnumerable<TDetail> details)
        {
            Master = master;

            Details = details.Aggregate( 
					ImmutableHashSet<TDetail>.Empty, (accu, current) => accu.Add(current) );
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
    

    public class PolicyAssignment : IEquatable<PolicyAssignment> {
            
        public AccessPolicy Source { get; private set; }
        public Either<LockAssignment,KeyAssignment> Target { get; private set; }

        public PolicyAssignment(AccessPolicy source, Either<LockAssignment,KeyAssignment> target) {
            this.Source = source;
            this.Target = target;
        }

        public bool Equals(PolicyAssignment other)
        {
            if (other == null)
                return false;

            return (this.Source.Equals(other.Source) && this.Target.Equals(other.Target));
        }
    }
}