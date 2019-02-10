namespace Lockbase.CoreDomain.Entities {

	public class Lock {

		public Lock(string id, string name, string appId, string extData) {
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
