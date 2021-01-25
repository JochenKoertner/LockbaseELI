using System;
using System.Reactive;
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
				OnNext(010, (MessageBuilder.Default + "LD")
					.BuildMessage(4711, nameof(LdOnEmptySystem))),
				OnCompleted<Message>(020)
			);

			var _ = new MessageBusInteractor(CreateLockSystem(), CreateLoggerFactory(), this.target, source);

			// Act
			var messageObserver = this.scheduler.Start(() => source, 0, 0, TimeSpan.FromSeconds(5).Ticks);

			// Assert 
			var expected = (MessageBuilder.Default + "LDR,OK").BuildStatement(4711, nameof(LdOnEmptySystem));
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
					.BuildMessage(4711, nameof(CkCLOnEmptySystem))),
				OnCompleted<Message>(020)
			);

			var _ = new MessageBusInteractor(CreateLockSystem(), CreateLoggerFactory(), this.target, source);

			// Act
			var messageObserver = this.scheduler.Start(() => source, 0, 0, TimeSpan.FromSeconds(5).Ticks);

			// Assert
			var expected = (MessageBuilder.Default
				+ "DK,040000jbookls,,,,"
				+ "DL,040000rbookls,,,,")
				.BuildStatement(4711, nameof(CkCLOnEmptySystem));
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
					.BuildMessage(4711, nameof(DKDLOnSystem))
				),
				OnCompleted<Message>(020)
			);

			var _ = new MessageBusInteractor(lockSystem, CreateLoggerFactory(), this.target, source);

			// Act
			var messageObserver = this.scheduler.Start(() => source, 0, 0, TimeSpan.FromSeconds(5).Ticks);

			// Assert
			var expected = (MessageBuilder.Default
				+ $"DK,040000jbookls,,,,{"103-1, Fender, Klaus\0".ToBase64()}"
				+ $"DL,040000rbookls,,,,{"105, Büro Vertrieb 2\0".ToBase64()}")
				.BuildStatement(4711, nameof(DKDLOnSystem));

			ReactiveAssert.AreElementsEqual(
				new Recorded<Notification<Statement>>[] {
					OnNext(010, expected),
					OnCompleted<Statement>(010) },
				this.target.Messages);
		}

		[Fact]
		public void ATAKOnSystem()
		{
			// Arrange
			var lockSystem = CreateLockSystem();
			lockSystem.SetValue(sys => sys
				.DefineStatement("CK,APP,103-1")
				.DefineStatement("CL,APP,105")
				.DefineStatement($"DK,040000jbookls,,,{"103-1, Fender, Klaus\0".ToBase64()}")
				.DefineStatement($"DL,040000rbookls,,,{"105, Büro Vertrieb 2\0".ToBase64()}")
			);

			var source = scheduler.CreateColdObservable(
				OnNext(010, (MessageBuilder.Default
					+ $"AT,000002oe1g25o,,20190212T080000Z/28800/DW(Mo+Tu+We+Th+Fr)/20190329T160000Z,20190401T070000Z/28800/DW(Mo+Tu+We+Th+Fr)/20191025T150000Z,20191028T080000Z/28800/DW(Mo+Tu+We+Th+Fr)/20200211T160000Z"
					+ $"AK,040000jbookls,000002oe1g25o,040000rbookls")
					.BuildMessage(4711, nameof(ATAKOnSystem))
				),
				OnCompleted<Message>(020)
			);

			var _ = new MessageBusInteractor(lockSystem, CreateLoggerFactory(), this.target, source);

			// Act
			var messageObserver = this.scheduler.Start(() => source, 0, 0, TimeSpan.FromSeconds(5).Ticks);

			// Assert
			var expected = (MessageBuilder.Default
				+ "ATR,000002oe1g25o,OK"
				+ "AKR,040000jbookls,OK")
				.BuildStatement(4711, nameof(ATAKOnSystem));

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
	}
}