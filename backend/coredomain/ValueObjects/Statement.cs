
using System;

namespace Lockbase.CoreDomain.ValueObjects
{

	public class Statement
	{
		

		public Statement(string topic, int sessionId, string statement)
		{
			Topic = topic;
			SessionId = sessionId;
			int index = statement.IndexOf(',');
			Head = statement.Substring(0, index);
			Tail = statement.Substring(index + 1);
		}

		public string Head { get; private set; }
		public string Tail { get; private set; }
		public string Topic { get; private set; }
		public int SessionId { get; private set; }
	}
}