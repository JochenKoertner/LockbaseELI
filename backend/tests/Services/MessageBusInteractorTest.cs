using System;
using System.Collections.Immutable;
using System.Reactive;
using System.Text;
using FakeItEasy;
using Lockbase.CoreDomain;
using Lockbase.CoreDomain.Aggregates;
using Lockbase.CoreDomain.Extensions;
using Lockbase.CoreDomain.Services;
using Lockbase.CoreDomain.ValueObjects;
using Lockbase.Tests.CoreDomain;
using Microsoft.Extensions.Logging;
using Microsoft.Reactive.Testing;
using Xunit;


namespace Lockbase.Tests.Services
{
	public class MessageBusInteractTest : ReactiveTest
	{
		private readonly TestScheduler scheduler;

		private readonly ITestableObserver<Statement> target;

		public MessageBusInteractTest() : base()
		{
			this.scheduler = new TestScheduler();
			this.target = this.scheduler.CreateObserver<Statement>();
		}

		[Fact]
		public void LdOnEmptySystem()
		{
			// Arrange 
			var source = scheduler.CreateColdObservable(
				OnNext(010, new Message() { text = "LD", correlationId = 4711, replyTo = nameof(LdOnEmptySystem) }),
				OnCompleted<Message>(020)
			);

			var _ = new MessageBusInteractor(CreateLockSystem(), CreateLoggerFactory(), this.target, source);

			// Act
			var messageObserver = this.scheduler.Start(() => source, 0, 0, TimeSpan.FromSeconds(5).Ticks);

			// Assert 
			var expected = new Statement(nameof(LdOnEmptySystem), 4711, "LDR,OK");
			ReactiveAssert.AreElementsEqual(
				new Recorded<Notification<Statement>>[] { OnNext(010, expected), OnCompleted<Statement>(010) },
				this.target.Messages);
		}

		[Fact]
		public void CkCLOnEmptySystem()
		{
			// Arrange 
			var source = scheduler.CreateColdObservable(
				OnNext(010, (MessageBuilder.Default 
					+ "CK,APP,103-1" 
					+ "CL,APP,105")
					.Build( 4711, nameof(CkCLOnEmptySystem))),
				OnCompleted<Message>(020)
			);

			var _ = new MessageBusInteractor(CreateLockSystem(), CreateLoggerFactory(), this.target, source);

			// Act
			var messageObserver = this.scheduler.Start(() => source, 0, 0, TimeSpan.FromSeconds(5).Ticks);

			// Assert DK,040000jbookls,,,,\nDL,040000rbookls,,,,\nDK,040000jbookls,,,,AA==
			var expected = new Statement(nameof(CkCLOnEmptySystem), 4711,
				"DK,040000jbookls,,,,\nDL,040000rbookls,,,,");
			ReactiveAssert.AreElementsEqual(
				new Recorded<Notification<Statement>>[] {
					OnNext(010, expected),
					OnCompleted<Statement>(010) },
				this.target.Messages);
		}

		[Fact]
		public void DKDLOnSystem()
		{
			// Arrange 

			var lockSystem = CreateLockSystem();
			lockSystem.SetValue(sys => sys
			   .DefineStatement("CK,APP,103-1")
			   .DefineStatement("CL,APP,105"));

			var source = scheduler.CreateColdObservable(
				OnNext(010, (MessageBuilder.Default 
					+ $"DK,040000jbookls,,,{"103-1, Fender, Klaus\0".ToBase64()}"
					+ $"DL,040000rbookls,,,{"105, Büro Vertrieb 2\0".ToBase64()}")
					.Build(4711,nameof(DKDLOnSystem))
				),
				OnCompleted<Message>(020)
			);

			var _ = new MessageBusInteractor(lockSystem, CreateLoggerFactory(), this.target, source);

			// Act
			var messageObserver = this.scheduler.Start(() => source, 0, 0, TimeSpan.FromSeconds(5).Ticks);

			// Assert 
			var expected = new Statement(nameof(DKDLOnSystem), 4711,
				$"DK,040000jbookls,,,,{"103-1, Fender, Klaus\0".ToBase64()}\nDL,040000rbookls,,,,{"105, Büro Vertrieb 2\0".ToBase64()}");
			ReactiveAssert.AreElementsEqual(
				new Recorded<Notification<Statement>>[] {
					OnNext(010, expected),
					OnCompleted<Statement>(010) },
				this.target.Messages);
		}

		private AtomicValue<LockSystem> CreateLockSystem() => new AtomicValue<LockSystem>(CreateEmptyLockSystem());

		private LockSystem CreateEmptyLockSystem() => LockSystem.Create(new Id("2020-01-23T16:14:35".FakeNow()));

		private ILoggerFactory CreateLoggerFactory()
		{
			var loggerFactory = A.Fake<ILoggerFactory>();
			var logger = A.Fake<ILogger<MessageBusInteractor>>();
			A.CallTo(() => loggerFactory.CreateLogger(A<string>._)).Returns(logger);
			return loggerFactory;
		}

		private class MessageBuilder {
			private readonly ImmutableList<string> messages;

			public static MessageBuilder Default => new MessageBuilder(ImmutableList<string>.Empty);

			private MessageBuilder(ImmutableList<string> messages) {
				this.messages=messages;
			}

			public MessageBuilder Add(string msg) => new MessageBuilder(this.messages.Add(msg));

			public static MessageBuilder operator +(MessageBuilder a, string b) => a.Add(b);
			
			public Message Build(int correlationId, string replyTo) 
			=> new Message() { text = String.Join("\n", this.messages), correlationId = correlationId, replyTo = replyTo };
		} 
	}
}