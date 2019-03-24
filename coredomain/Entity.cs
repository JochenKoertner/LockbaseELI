using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Lockbase.CoreDomain
{
    public abstract class Entity : IEquatable<Entity>
    {     
        public string Id { get; private set; }

        public Entity(string id)
        {
            Id = id;            
        }

        public bool Equals(Entity other)
        {
            if (other.Equals(null))
                return false;

            return (this.Id == other.Id);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !((obj is Entity) || (obj is string)))
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            if (obj.GetType() == typeof(string))
                return (string)obj == this.Id;

            if (this.GetType() != obj.GetType())
                return false;

            Entity item = (Entity)obj;

            return item.Id == this.Id;
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode() ^ 31; 
            // XOR for random distribution (http://blogs.msdn.com/b/ericlippert/archive/2011/02/28/guidelines-and-rules-for-gethashcode.aspx)
        }

        public static bool operator ==(Entity left, Entity right)
        {
            if (Object.Equals(left, null))
                return (Object.Equals(right, null)) ? true : false;
            else
                return left.Equals(right);
        }

        public static bool operator !=(Entity left, Entity right)
        {
            return !(left == right);
        }
    }
}