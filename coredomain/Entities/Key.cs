using System; 

namespace Lockbase.CoreDomain.Entities {

	// Repräsentert eine 'Schlüssel' Entität
	public class Key : IEquatable<Key> {

		public Key(string id, string name, string appId, string extData) {

			if (string.IsNullOrWhiteSpace(id)) throw new ArgumentNullException(nameof(id));
			Id = id;
			Name = name;
			AppId = appId;
			ExtData = extData;	
		}

		public string Id { get; private set;  } 
		public string Name { get; private set;  } 
		public string AppId { get; private set;  } 
		public string ExtData { get; private set;  } 

		public bool Equals(Key other)
        {
            if (other == null)
                return false;

            return (this.Id == other.Id);
        }
	}
}
