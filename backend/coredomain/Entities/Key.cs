using System;
using System.Diagnostics;
using Lockbase.CoreDomain.Extensions;

namespace Lockbase.CoreDomain.Entities {

	// Repräsentert eine 'Schlüssel' Entität
	[DebuggerDisplay("{debugDescription,nq} ({Name,nq},{ExtData,nq})")]
	public class Key: Entity, IEquatable<Key> {

		public Key(string id, string name, string appId, string extData = ""):base(id) {
			Name = name;
			AppId = appId;
			ExtData = extData;	
		}

		public string Name { get; private set;  } 
		public string AppId { get; private set;  } 
		public string ExtData { get; private set;  } 

		public bool Equals(Key other) => base.Equals(other);

		private string debugDescription 
		=> String.Join(',', new []{"DK", this.Id, this.AppId, 
			$"{this.Name}, {this.ExtData}\0".ToBase64().Shorten()});
		
	}
}
