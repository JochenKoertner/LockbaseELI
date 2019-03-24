using System;
using System.Linq;

using Xunit;

using Lockbase.CoreDomain;
using Lockbase.CoreDomain.Entities;
using Lockbase.CoreDomain.Aggregates;
using Lockbase.CoreDomain.ValueObjects;

namespace Lockbase.ui.UnitTest.CoreDomain {

	public class LockSystemTest {

        const string TorWestId = "000000t00nuiu";
        const string KlausFenderId = "000000hqvs1lo";

        const string WerktagPolicyId = "000002oe1g25o";

        public LockSystem system; 

        public LockSystemTest()
        {
            TimePeriodDefinition timePeriodDefinition = "20181231T230000Z/63072000";
			AccessPolicy accessPolicy = new AccessPolicy("000002oe1g25o", new NumberOfLockings(12,45), new []{timePeriodDefinition});
            system = new LockSystem()
                .AddLock(new Lock(TorWestId, "W1",null, "Tor West"))
                .AddKey(new Key(KlausFenderId,"103-1",null, "Fender, Klaus"))
                .AddAccessPolicy(accessPolicy);
        }

		[Fact]
		public void TestFoundEntities() {

            Assert.Single(system.Locks, _ => _ == TorWestId );
            Assert.Single(system.Keys, _ => _ == KlausFenderId );
            Assert.Single(system.Policies, _ => _ == WerktagPolicyId );
		}

        [Fact]
        public void TestAssignLockToKey() {

            var torwest = system.QueryLock(TorWestId);
            var klaus = system.QueryKey(KlausFenderId);
            var werktags = system.QueryPolicy(WerktagPolicyId);

            Assert.NotNull(torwest);
            Assert.NotNull(klaus);
            Assert.NotNull(werktags);

            Assert.Empty(system.QueryPolicies(torwest, klaus));

            system = system.AddAssignment(torwest, werktags, klaus.Yield());

            var policies = system.QueryPolicies(torwest, klaus);

            Assert.Single(policies);
            Assert.True( policies.First() == WerktagPolicyId);
        }

	}
}
