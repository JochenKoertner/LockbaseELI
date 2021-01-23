using System;
using System.Diagnostics;
using Lockbase.CoreDomain.Extensions;

namespace Lockbase.CoreDomain.ValueObjects
{
	[DebuggerDisplay("{debugDescription,nq} (#{JobId,nq},{Topic,nq})")]

	public readonly struct Statement
	{
		public readonly string Head;
		public readonly string Tail;
		public readonly string Topic;
		public readonly int JobId;

		public Statement(string topic, int sessionId, string statement) 
			=> (Topic, JobId, Head, Tail) = 
				(topic, sessionId, 
				statement.Substring(0, statement.IndexOf(',')),
				statement.Substring(statement.IndexOf(',') + 1));

		public string Message => $"{Head},{Tail}";

		private string debugDescription => String.Join(',', new[]{this.Head, this.Tail.Shorten()});

	}
}