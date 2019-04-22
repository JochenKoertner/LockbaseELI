using System;

namespace ui.Models {

	public class GroupOrDoor {
	}

	public class DoorInfo : GroupOrDoor
	{
		public string lockId { get; set; }
		public string value { get; set; }
		public string label { get; set; }
		public string building { get; set; }
		public string image { get; set; }
		public string color { get; set; }
	}

	public class GroupInfo : GroupOrDoor
	{
		public string type { get; set; }
		public string name { get; set; }

		public DoorInfo[] items { get; set; }
	}
}
