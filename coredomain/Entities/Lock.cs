using System; 

namespace Lockbase.CoreDomain.Entities {

	// Repräsentert eine 'Schloss' Entität
	public class Lock: Entity, IEquatable<Lock> {

		public Lock(string id, string name, string appId, string extData):base(id) 
		{
			Name = name;
			AppId = appId;
			ExtData = extData;	
		}

		public string Name { get; private set;  } 
		public string AppId { get; private set;  } 
		public string ExtData { get; private set;  }

        public bool Equals(Lock other) => base.Equals(other);
    }
}
