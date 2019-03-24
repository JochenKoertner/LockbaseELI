using System; 

namespace Lockbase.CoreDomain.Entities {

	// Repräsentert eine 'Schloss' Entität
	public class Lock : IEquatable<Lock> {

		public Lock(string id, string name, string appId, string extData) {

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

		public bool Equals(Lock other)
        {
            if (null == other)
                return false;

            return (this.Id == other.Id);
        }

		public override bool Equals(object other) {
			if (other == null)
				return false;
			if (other is Lock)
				return this.Equals((Lock)other);
			if (other is String) 
				return this.Id.Equals((String)other);

			return false;
		}

		public override int GetHashCode() {
			return this.Id.GetHashCode();
		}

        public static bool operator ==(Lock a, String b) => Object.Equals(a, b);

        public static bool operator !=(Lock a, String b) => !Object.Equals(a, b);
    }
}
