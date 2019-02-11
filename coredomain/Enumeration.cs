using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Lockbase.CoreDomain
{
	public abstract class Enumeration<T> : IComparable
		where T : Enumeration<T>
	{
		public int Id { get; }
		public string Name { get; }

		protected Enumeration(int id, string name)
		{
			this.Id = id;
			this.Name = name;
		}

		public override string ToString() => this.Name;
		public override int GetHashCode() => this.Id.GetHashCode();

		public override bool Equals(object other)
		{
			var otherValue = other as Enumeration<T>;

			if (otherValue == null)
				return false;

			var typeMatches = this.GetType() == other.GetType();
			var valueMatches = this.Id.Equals(otherValue.Id);

			return typeMatches && valueMatches;
		}

		public static IEnumerable<T> GetAll()
		{
			var type = typeof(T);
			var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

			foreach (var info in fields)
			{
				if (info.GetValue(null) is T locatedValue)
					yield return locatedValue;
			}
		}

		public static implicit operator int(Enumeration<T> instance)
		{
			return instance.Id;
		}		
		
		public static implicit operator Enumeration<T>(int value)
		{
			var matchingItem = GetAll().FirstOrDefault(item => item.Id == value);
			if (matchingItem == null)
				throw new InvalidOperationException($"'{value}' is not a valid for {typeof(T)}");

			return matchingItem;
		}
		
		public int CompareTo(object other) => this.Id.CompareTo(((Enumeration<T>)other).Id);
	}
}
