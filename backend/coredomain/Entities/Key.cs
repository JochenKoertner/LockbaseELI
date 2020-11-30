using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Lockbase.CoreDomain.Extensions;

namespace Lockbase.CoreDomain.Entities
{

	// Repräsentert eine 'Schlüssel' Entität
	[DebuggerDisplay("{debugDescription,nq} ({Name,nq},{ExtData,nq})")]
	public class Key : Entity, IEquatable<Key>
	{

		public Key(string id, string name, string appId, string extData = "") : base(id)
		{
			Name = name;
			AppId = appId;
			ExtData = extData;
			Func = string.Empty;
		}

		public string Name { get; private set; }
		public string Func { get; private set; }
		public string AppId { get; private set; }
		public string ExtData { get; private set; }

		public bool Equals(Key other) => base.Equals(other);

		private string debugDescription
		=> String.Join(',', new[]{"DK", this.Id, this.Func, this.AppId,
			$"{this.Name}, {this.ExtData}\0".ToBase64().Shorten()});

	}

	public class KeyComparer : IEqualityComparer<Key>
	{
		public bool Equals([AllowNull] Key x, [AllowNull] Key y)
		{
			return 
				x.Name.Equals(y.Name) && 
				x.Func.Equals(y.Func) &&
				x.AppId.Equals(y.AppId) &&
				x.ExtData.Equals(y.ExtData);
		}

		public int GetHashCode([DisallowNull] Key value)
		{
			return 
				value.Name.GetHashCode() ^ 
				value.Func.GetHashCode() ^ 
				value.AppId.GetHashCode() ^ 
				value.ExtData.GetHashCode();
		}
	}
}
