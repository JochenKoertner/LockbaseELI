using System; 

namespace Lockbase.CoreDomain.Entities {

	// Repräsentert eine 'Schloss' Entität
	public class Lock {

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
	}
}
