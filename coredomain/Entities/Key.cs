using System; 

namespace Lockbase.CoreDomain.Entities {

	// Repräsentert eine 'Schlüssel' Entität
	public class Key: Entity, IEquatable<Key> {

		public Key(string id, string name, string appId, string extData):base(id) {
			Name = name;
			AppId = appId;
			ExtData = extData;	
		}

		public string Name { get; private set;  } 
		public string AppId { get; private set;  } 
		public string ExtData { get; private set;  } 

		public bool Equals(Key other) => base.Equals(other);

	}
}
