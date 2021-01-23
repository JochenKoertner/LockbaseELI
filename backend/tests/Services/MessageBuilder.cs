using System;
using System.Collections.Immutable;
using System.Text;
using Lockbase.CoreDomain.Services;
using Lockbase.CoreDomain.ValueObjects;

namespace Lockbase.Tests.Services
{
	internal struct MessageBuilder
	{
		private ImmutableList<string> Messages { get; init; }

		public static MessageBuilder Default => new MessageBuilder(ImmutableList<string>.Empty);

		private MessageBuilder(ImmutableList<string> messages) { 
			this.Messages = messages;
		}
		
		public static MessageBuilder operator +(MessageBuilder builder, string msg) => new MessageBuilder(builder.Messages.Add(msg));

		public Message BuildMessage(int correlationId, string replyTo)
		=> new Message() { text = String.Join("\n", this.Messages), correlationId = correlationId, replyTo = replyTo };

		public Statement BuildStatement(int correlationId, string replyTo)
		=> new Statement(replyTo, correlationId, String.Join("\n", this.Messages));
	}
}