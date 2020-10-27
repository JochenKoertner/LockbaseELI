using System;
using System.Diagnostics;
using System.Linq;
using Lockbase.CoreDomain.Extensions;

namespace Lockbase.CoreDomain.Entities {

	// Repräsentert eine 'Schloss' Entität
	[DebuggerDisplay("{debugDescription,nq} ({Name,nq},{ExtData,nq})")]
	public class Lock: Entity, IEquatable<Lock> {

		public Lock(string id, string name, string appId, string extData=""):base(id) 
		{
			Name = name;
			AppId = appId;
			ExtData = extData;	
		}

		public string Name { get; private set;  } 
		public string AppId { get; private set;  } 
		public string ExtData { get; private set;  }

        public bool Equals(Lock other) => base.Equals(other);
		private string debugDescription 
		=> String.Join(',', new []{"DL", this.Id, this.AppId, 
			$"{this.Name}, {this.ExtData}\0".ToBase64().Shorten()});

	

    }
}
