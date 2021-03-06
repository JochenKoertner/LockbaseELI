using System;
using System.Linq;

using Xunit;

using Lockbase.CoreDomain;
using Lockbase.CoreDomain.Entities;
using Lockbase.CoreDomain.Aggregates;
using Lockbase.CoreDomain.ValueObjects;
using System.IO;
using System.Collections.Immutable;
using Lockbase.CoreDomain.Extensions;
using Lockbase.CoreDomain.Enumerations;
using Lockbase.CoreDomain.Services;

namespace Lockbase.Tests.CoreDomain
{

	public class LockSystemTest
	{

		const string TorWestId = "000000t00nuiu";
		const string OfficeId = "040000t00nuiu";
		const string SalesId = "080000t00nuiu";

		const string KlausFenderId = "000000hqvs1lo";

		const string WerktagPolicyId = "000002oe1g25o";

		private readonly LockSystem system;

		public LockSystemTest()
		{
			TimePeriodDefinition timePeriodDefinition = "20190211T080000Z/28800/DW(Mo+Tu+We+Th+Fr)/20190329T160000Z";
			AccessPolicy accessPolicy = new AccessPolicy("000002oe1g25o", new NumberOfLockings(12, 45), new[] { timePeriodDefinition });
			this.system = CreateEmptyLockSystem()
				.AddLock(new Lock(TorWestId, "W1", null, "Tor West"))
				.AddKey(new Key(KlausFenderId, "103-1", null, "Fender, Klaus"))
				.AddAccessPolicy(accessPolicy);
		}

		private LockSystem CreateEmptyLockSystem() => LockSystem.Create(new Id("2020-01-23T16:14:35".FakeNow()));

		[Fact]
		public void TestFoundEntities()
		{

			Assert.Single(system.Locks, _ => _ == TorWestId);
			Assert.Single(system.Keys, _ => _ == KlausFenderId);
			Assert.Single(system.Policies, _ => _ == WerktagPolicyId);
		}

		[Fact]
		public void TestAssignLockToKeySingle()
		{

			var torwest = system.QueryLock(TorWestId);
			var klaus = system.QueryKey(KlausFenderId);
			var werktags = system.QueryPolicy(WerktagPolicyId);

			Assert.NotNull(torwest);
			Assert.NotNull(klaus);
			Assert.NotNull(werktags);

			Assert.Empty(system.QueryPolicies(torwest, klaus));

			var newSystem = system.AddAssignment(torwest, werktags, klaus.Yield());

			var policies = newSystem.QueryPolicies(torwest, klaus);

			Assert.Single(policies);
			Assert.True(policies.First() == WerktagPolicyId);
		}

		[Fact]
		public void TestAssignKeyToLockSingle()
		{

			var torwest = system.QueryLock(TorWestId);
			var klaus = system.QueryKey(KlausFenderId);
			var werktags = system.QueryPolicy(WerktagPolicyId);

			Assert.NotNull(torwest);
			Assert.NotNull(klaus);
			Assert.NotNull(werktags);

			Assert.Empty(system.QueryPolicies(torwest, klaus));

			var newSystem = system.AddAssignment(klaus, werktags, torwest.Yield());

			var policies = newSystem.QueryPolicies(torwest, klaus);

			Assert.Single(policies);
			Assert.True(policies.First() == WerktagPolicyId);
		}

		[Fact]
		public void TestKeyToMultipleLockAssigments()
		{

			var torwest = system.QueryLock(TorWestId);
			var klaus = system.QueryKey(KlausFenderId);
			var werktags = system.QueryPolicy(WerktagPolicyId);

			var office = new Lock(OfficeId, "Office", null, "Office");
			var sales = new Lock(SalesId, "Sales", null, "Sales");

			var newSystem = system
				.AddLock(office)
				.AddLock(sales)
				.AddAssignment(torwest, werktags, klaus.Yield())
				.AddAssignment(office, werktags, klaus.Yield())
				.AddAssignment(sales, werktags, klaus.Yield());


			Assert.Single(newSystem.QueryPolicies(torwest, klaus));
			Assert.Single(newSystem.QueryPolicies(office, klaus));
			Assert.Single(newSystem.QueryPolicies(sales, klaus));
		}

		[Fact]
		public void TestMulitpleLocksToSingleKeyAssigments()
		{

			var torwest = system.QueryLock(TorWestId);
			var klaus = system.QueryKey(KlausFenderId);
			var werktags = system.QueryPolicy(WerktagPolicyId);

			var office = new Lock(OfficeId, "Office", null, "Office");
			var sales = new Lock(SalesId, "Sales", null, "Sales");

			var newSystem = system
				.AddLock(office)
				.AddLock(sales)
				.AddAssignment(klaus, werktags, new[] { torwest, office, sales });

			Assert.Single(newSystem.QueryPolicies(torwest, klaus));
			Assert.Single(newSystem.QueryPolicies(office, klaus));
			Assert.Single(newSystem.QueryPolicies(sales, klaus));
		}

		[Fact]
		public void TestHasAccess()
		{

			var torwest = system.QueryLock(TorWestId);
			var klaus = system.QueryKey(KlausFenderId);
			var werktags = system.QueryPolicy(WerktagPolicyId);

			// "20190211T080000Z/28800/DW(Mo+Tu+We+Th+Fr)/20190329T160000Z";
			//  2019-02-11 08:00 - 16:00 bis 2019-03-29   (Mo,Di,Mi,Do,Fr) 
			var newSystem = system
				.AddAssignment(klaus, werktags, new[] { torwest });

			var friday = new DateTime(2019, 2, 15, 12, 0, 0);  // Freitag Mittag ok 
			Assert.Equal(EventType.Authorized_Access,
				newSystem.HasAccess(klaus, torwest, friday));
		}

		[Fact]
		public void TestDefineLock()
		{
			var lockSystem = CreateEmptyLockSystem().DefineLock("000000t00nuiu,,,MTAwLCBNZWV0aW5nIFJvb20sIEFkbWluaXN0cmF0aW9uAA==");

			var door = lockSystem.QueryLock("000000t00nuiu");

			Assert.NotNull(door);
			Assert.Empty(door.AppId);
			Assert.Empty(door.Name);
			Assert.Equal("100, Meeting Room, Administration", door.ExtData);
		}


		[Fact]
		public void TestDefineKey()
		{
			var lockSystem = CreateEmptyLockSystem().DefineKey("000000hqvs1lo,,,MTAzLTEsIEZlbmRlciwgS2xhdXMA");

			var @key = lockSystem.QueryKey("000000hqvs1lo");

			Assert.NotNull(@key);
			Assert.Empty(@key.AppId);
			Assert.Empty(@key.Name);
			Assert.Equal("103-1, Fender, Klaus", @key.ExtData);
		}

		[Fact]
		public void TestDefinePolicy()
		{
			var lockSystem = CreateEmptyLockSystem().DefinePolicy(
				"000002oe1g25o,,20190212T080000Z/28800/DW(Mo+Tu+We+Th+Fr)/20190329T160000Z,20190401T070000Z/28800/DW(Mo+Tu+We+Th+Fr)/20191025T150000Z,20191028T080000Z/28800/DW(Mo+Tu+We+Th+Fr)/20200211T160000Z");

			var policy = lockSystem.QueryPolicy("000002oe1g25o");

			Assert.NotNull(policy);
			Assert.Null(policy.NumberOfLockings);
			Assert.Equal(3, policy.TimePeriodDefinitions.Count());
		}

		[Fact]
		public void TestDefineAssignmentKey()
		{

			var lockSystem = CreateEmptyLockSystem()
				.DefineKey("000000hqvs1lo,,,MTAzLTEsIEZlbmRlciwgS2xhdXMA")
				.DefineLock("0c0000t00nuiu,,,MTAzLCBBY2NvdW50aW5nLCBBZG1pbmlzdHJhdGlvbgA=")
				.DefineLock("580000t00nuiu,,,WjEsIEVudHJhbmNlIFdlc3QsIEFkbWluaXN0cmF0aW9uAA==")
				.DefinePolicy("000002oe1g25o,,20190212T080000Z/28800/DW(Mo+Tu+We+Th+Fr)/20190329T160000Z,20190401T070000Z/28800/DW(Mo+Tu+We+Th+Fr)/20191025T150000Z,20191028T080000Z/28800/DW(Mo+Tu+We+Th+Fr)/20200211T160000Z")
				.DefineAssignmentKey("000000hqvs1lo,000002oe1g25o,0c0000t00nuiu,580000t00nuiu");

			var @lock = lockSystem.QueryLock("580000t00nuiu");
			var key = lockSystem.QueryKey("000000hqvs1lo");

			var policy = lockSystem.QueryPolicies(@lock, key).SingleOrDefault();

			Assert.NotNull(policy);
			Assert.Null(policy.NumberOfLockings);
			Assert.Equal(3, policy.TimePeriodDefinitions.Count());
		}

		[Fact]
		public void TestDefineAssignmentLock()
		{

			var lockSystem = CreateEmptyLockSystem()
				.DefineKey("000000hqvs1lo,,,MTAzLTEsIEZlbmRlciwgS2xhdXMA")
				.DefineLock("0c0000t00nuiu,,,MTAzLCBBY2NvdW50aW5nLCBBZG1pbmlzdHJhdGlvbgA=")
				.DefinePolicy("000002oe1g25o,,20190212T080000Z/28800/DW(Mo+Tu+We+Th+Fr)/20190329T160000Z,20190401T070000Z/28800/DW(Mo+Tu+We+Th+Fr)/20191025T150000Z,20191028T080000Z/28800/DW(Mo+Tu+We+Th+Fr)/20200211T160000Z")
				.DefineAssignmentLock("0c0000t00nuiu,000002oe1g25o,000000hqvs1lo");

			var @lock = lockSystem.QueryLock("0c0000t00nuiu");
			var key = lockSystem.QueryKey("000000hqvs1lo");

			var policy = lockSystem.QueryPolicies(@lock, key).SingleOrDefault();

			Assert.NotNull(policy);
			Assert.Null(policy.NumberOfLockings);
			Assert.Equal(3, policy.TimePeriodDefinitions.Count());
		}

		[Fact]
		public void TestDefineAssignmentDuplicate()
		{

			var lockSystem = CreateEmptyLockSystem()
				.DefineKey("000000hqvs1lo,,,MTAzLTEsIEZlbmRlciwgS2xhdXMA")
				.DefineLock("0c0000t00nuiu,,,MTAzLCBBY2NvdW50aW5nLCBBZG1pbmlzdHJhdGlvbgA=")
				.DefinePolicy("000002oe1g25o,,20190212T080000Z/28800/DW(Mo+Tu+We+Th+Fr)/20190329T160000Z,20190401T070000Z/28800/DW(Mo+Tu+We+Th+Fr)/20191025T150000Z,20191028T080000Z/28800/DW(Mo+Tu+We+Th+Fr)/20200211T160000Z")
				.DefineAssignmentLock("0c0000t00nuiu,000002oe1g25o,000000hqvs1lo")
				.DefineAssignmentLock("0c0000t00nuiu,000002oe1g25o,000000hqvs1lo");

			var @lock = lockSystem.QueryLock("0c0000t00nuiu");
			var key = lockSystem.QueryKey("000000hqvs1lo");

			var policies = lockSystem.QueryPolicies(@lock, key);
			Assert.NotEmpty(policies);
		}

		[Fact]
		public void TestDuplicateHashSet()
		{
			var sourceA = new AccessPolicy("id", new NumberOfLockings(1, 2), Enumerable.Empty<TimePeriodDefinition>());
			var sourceB = new AccessPolicy("id", new NumberOfLockings(1, 2), Enumerable.Empty<TimePeriodDefinition>());

			var key = new Key("idKey", "key", "appId", "Hugo");
			var @lock = new Lock("idLock", "lock", "appId", "Tor West");

			var eitherA = new Either<LockAssignment, KeyAssignment>(new KeyAssignment(key, new[] { @lock }));
			var eitherB = new Either<LockAssignment, KeyAssignment>(new KeyAssignment(key, new[] { @lock }));

			Assert.Equal(eitherA, eitherB);

			var assignA = new PolicyAssignment(sourceA,
					new Either<LockAssignment, KeyAssignment>(new KeyAssignment(key, new[] { @lock })));
			var assignB = new PolicyAssignment(sourceA,
					new Either<LockAssignment, KeyAssignment>(new KeyAssignment(key, new[] { @lock })));

			var setAssignments = ImmutableHashSet<PolicyAssignment>.Empty.Add(assignA).Add(assignB);
			Assert.NotEmpty(setAssignments);
		}

		[Fact]
		public void TestCreateKey()
		{
			var system = CreateEmptyLockSystem()
				.DefineStatement("CK,APP,103-1")
				.DefineStatement("CK,APP,103-3");
			Assert.Equal(2, system.Keys.Count());
			var key103_1 = system.Keys.First(x => x.Name == "103-1");
			Assert.Equal("040000jbookls", key103_1.Id);
			Assert.Equal("APP", key103_1.AppId);
			var key103_3 = system.Keys.First(x => x.Name == "103-3");
			Assert.Equal("080000jbookls", key103_3.Id);
			Assert.Equal("APP", key103_3.AppId);
		}

		[Fact]
		public void TestCreateLock()
		{
			var system = CreateEmptyLockSystem()
				.DefineStatement("CL,APP,105")
				.DefineStatement("CL,APP,Z1");
			Assert.Equal(2, system.Locks.Count());
			var lock105 = system.Locks.First(x => x.Name == "105");
			Assert.Equal("040000rbookls", lock105.Id);
			Assert.Equal("APP", lock105.AppId);
			var lockZ1 = system.Locks.First(x => x.Name == "Z1");
			Assert.Equal("080000rbookls", lockZ1.Id);
			Assert.Equal("APP", lockZ1.AppId);
		}

		[Fact]
		public void TestDefineKeyAfterCreate()
		{
			const string keyName = "103-1";
			var extData = $"{keyName}, Fender, Klaus\0".ToBase64();
			var system = CreateEmptyLockSystem()
				.DefineStatement($"CK,APP,{keyName}");
			Func<string,string> getId = name => system.Keys.SingleOrDefault( key => key.Name == name)?.Id;

			system = system
				.DefineStatement($"DK,{getId(keyName)},,,{extData}");

			Assert.Equal(1, system.Keys.Count());
			var key_103_1 = system.Keys.First(x => x.Name == keyName);
			Assert.Equal("040000jbookls", key_103_1.Id);
			Assert.Equal(keyName, key_103_1.Name);
			Assert.Equal("APP", key_103_1.AppId);
			Assert.Equal($"{keyName}, Fender, Klaus", key_103_1.ExtData);
		}
		[Fact]
		public void TestDefineLockAfterCreate()
		{
			const string lockName = "100";
			var extData = $"{lockName}, Meeting Room, Administration\0".ToBase64();
			var system = CreateEmptyLockSystem()
				.DefineStatement($"CL,APP,{lockName}");
			Func<string, string> getId = name => system.Locks.SingleOrDefault(@lock => @lock.Name == name)?.Id;

			system = system
				.DefineStatement($"DL,{getId(lockName)},,,{extData}");

			Assert.Equal(1, system.Locks.Count());
			var lock_100 = system.Locks.First(x => x.Name == lockName);
			Assert.Equal("040000rbookls", lock_100.Id);
			Assert.Equal(lockName, lock_100.Name);
			Assert.Equal("APP", lock_100.AppId);
			Assert.Equal($"{lockName}, Meeting Room, Administration", lock_100.ExtData);
		}

		[Fact]
		public void TestDefineLockSystem()
		{
			var lines = File.ReadAllLines("samples/ELIApp2Drv.txt");
			var lockSystem = lines.Aggregate(
				seed: CreateEmptyLockSystem(),
				func: (system, line) => system.DefineStatement(line)
			);

			Assert.Equal(16, lockSystem.Keys.Count());
			Assert.Equal(13, lockSystem.Locks.Count());
			Assert.Equal(5, lockSystem.Policies.Count());

			var policiy = lockSystem.QueryPolicies(
					lockSystem.QueryLock("0g0000t00nuiu"),
					lockSystem.QueryKey("7s0000l00nuiu")).SingleOrDefault();

			Assert.NotNull(policiy);
			Assert.Equal("080002uc1k25o", policiy.Id);
		}

		[Fact]
		public void TestDeltaLockNewSystem()
		{
			const string lockName = "100";
			const string keyName = "103-1";

			var system_t0 = CreateEmptyLockSystem();
			var system_t1 = system_t0
				.DefineStatement($"CL,APP,{lockName}")
				.DefineStatement($"CK,APP,{keyName}");

			var newEntities = LockSystem.CreatedEntities(system_t0, system_t1);
			Assert.Equal(2, newEntities.Count());
			Assert.Equal(lockName, newEntities.OfType<Lock>().Single().Name);
			Assert.Equal(keyName, newEntities.OfType<Key>().Single().Name);

			var oldEntities = LockSystem.RemovedEntities(system_t1, system_t0);
			Assert.Equal(2, oldEntities.Count());
			Assert.Equal(lockName, oldEntities.OfType<Lock>().Single().Name);
			Assert.Equal(keyName, oldEntities.OfType<Key>().Single().Name);
		}

	}
}
