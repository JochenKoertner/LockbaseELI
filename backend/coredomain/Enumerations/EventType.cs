namespace Lockbase.CoreDomain.Enumerations
{
    public sealed class EventType : Enumeration<EventType>
	{
		public string Category { get; }
		public string Description  { get; }
		private EventType(int id, string name, string category, string description)
			: base(id, name)
		{
			this.Category = category;
			this.Description = description;
		}

		public static readonly EventType Unauthorized_Access = new EventType(0, "ESUA", "security", "Es wurde der Zutritt mit einem unberechtigen Schlüssel versucht");
		public static readonly EventType Battery_Low = new EventType(1, "ETBL", "technical", "Der Ladestand der Batterie ist niedrig");
		public static readonly EventType Power_Off = new EventType(2, "ETPO", "technical", "Die Stromversorgung ist unterbrochen");
		public static readonly EventType Maintenance = new EventType(3, "ETMR", "technical", "Das Gerät benötigt Wartung");
		public static readonly EventType Authorized_Access = new EventType(4, "EAAA", "access", "Einem berechtigten Schlüssel würde Zutritt gewährt");
		
	}
}