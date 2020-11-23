using System;
using System.Reactive;
using FakeItEasy;
using Lockbase.CoreDomain;
using Lockbase.CoreDomain.Aggregates;
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

		private AtomicValue<LockSystem> CreateLockSystem() => new AtomicValue<LockSystem>(CreateEmptyLockSystem());

		private LockSystem CreateEmptyLockSystem() => LockSystem.Create(new Id("2020-01-23T16:14:35".FakeNow()));

		private ILoggerFactory CreateLoggerFactory()
		{
			var loggerFactory = A.Fake<ILoggerFactory>();
			var logger = A.Fake<ILogger<MessageBusInteractor>>();
			A.CallTo(() => loggerFactory.CreateLogger(A<string>._)).Returns(logger);
			return loggerFactory;
		}
	}
}