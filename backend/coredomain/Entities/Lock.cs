using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Lockbase.CoreDomain.Extensions;

namespace Lockbase.CoreDomain.Entities
{

	// Repräsentert eine 'Schloss' Entität
	[DebuggerDisplay("{debugDescription,nq} ({Name,nq},{ExtData,nq})")]
	public class Lock : Entity, IEquatable<Lock>
	{

		public Lock(string id, string name, string appId, string extData = "") : base(id)
		{
			Name = name;
			AppId = appId;
			ExtData = extData;
			Func = string.Empty;
		}

		public string Name { get; private set; }
		public string AppId { get; private set; }
		public string Func { get; private set; }
		public string ExtData { get; private set; }

		public bool Equals(Lock other) => base.Equals(other);
		private string debugDescription
		=> String.Join(',', new[]{"DL", this.Id, this.Func, this.AppId,
			$"{this.Name}, {this.ExtData}\0".ToBase64().Shorten()});
	}
	public class LockComparer : IEqualityComparer<Lock>
	{
		public bool Equals([AllowNull] Lock x, [AllowNull] Lock y)
		{
			return
				x.Name.Equals(y.Name) &&
				x.Func.Equals(y.Func) &&
				x.AppId.Equals(y.AppId) &&
				x.ExtData.Equals(y.ExtData);
		}

		public int GetHashCode([DisallowNull] Lock value)
		{
			return
				value.Name.GetHashCode() ^
				value.Func.GetHashCode() ^
				value.AppId.GetHashCode() ^
				value.ExtData.GetHashCode();
		}
	}
}
