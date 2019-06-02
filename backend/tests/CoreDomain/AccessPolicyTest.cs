using System;
using System.Linq;

using Xunit; 
using Lockbase.CoreDomain.Entities;
using Lockbase.CoreDomain.ValueObjects;

namespace Lockbase.ui.UnitTest.CoreDomain {

	public class AccessPolicyTest {

		[Fact]
		public void TestCreation() {
			TimePeriodDefinition timePeriodDefinition = "20181231T230000Z/63072000";
			AccessPolicy entity = new AccessPolicy("000002oe1g25o", new NumberOfLockings(12,45), new []{timePeriodDefinition});
			Assert.Equal("000002oe1g25o", entity.Id);
			Assert.Equal(12, entity.NumberOfLockings.Minimum);
			Assert.Equal(45, entity.NumberOfLockings.Maximum);
			Assert.Equal(63072000, entity.TimePeriodDefinitions.First().Duration);
		}
	}
}
