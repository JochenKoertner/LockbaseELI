
using System;
using Lockbase.CoreDomain.Entities;

namespace Lockbase.CoreDomain.ValueObjects  {
    
    public class PolicyAssignment : IEquatable<PolicyAssignment> {
            
        public AccessPolicy Source { get; private set; }
        public Either<Lock,Key> Target { get; private set; }

        public PolicyAssignment(AccessPolicy source, Either<Lock,Key> target) {
            this.Source = source;
            this.Target = target;
        }

        public bool Equals(PolicyAssignment other)
        {
            if (other == null)
                return false;

            if (this.Source == other.Source && this.Target == other.Target)
                return true;
             else
                 return false;
        }
    }
}