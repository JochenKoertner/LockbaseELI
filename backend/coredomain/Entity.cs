using System;

namespace Lockbase.CoreDomain
{
	public abstract class Entity : IEquatable<Entity>
	{
		public string Id { get; init; }

		public Entity(string id)
		{
			if (string.IsNullOrWhiteSpace(id)) throw new ArgumentNullException(nameof(id));
			Id = id;
		}

		public bool Equals(Entity other)
		{
			if (null == other)
				return false;

			return (this.Id == other.Id);
		}

		public override bool Equals(object obj)
		{
			if (obj == null || !((obj is Entity) || (obj is string)))
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

		public static bool operator ==(Entity a, String b) => Object.Equals(a, b);

		public static bool operator !=(Entity a, String b) => !Object.Equals(a, b);
	}
}