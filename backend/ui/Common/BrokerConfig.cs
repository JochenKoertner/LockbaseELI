namespace ui.Common
{
	public class BrokerConfig
	{
		internal const string KEY = "broker";
		
		public string HostName { get; } = "localhost";
		public int Port { get; } = 1883;
		public string User { get; } = "Bob";
		public string Topic { get; } = "channel";
	}
}