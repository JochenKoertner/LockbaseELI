using System;
using Xunit; 

using Lockbase.CoreDomain.Entities;
using Lockbase.CoreDomain.Enumerations;
using Lockbase.CoreDomain.ValueObjects;
using System.Linq;

namespace Lockbase.ui.UnitTest.CoreDomain {

	public class RecurrenceRuleTimeTest {

		[Fact]
		public void TestSimpleSecond() 
		{
			RecurrenceRuleTime rule = "s";
			Assert.Equal(1, rule.Multiplier); 
			Assert.Equal(TimeInterval.Second, rule.Frequency);
			Assert.Equal(60, rule.Times.Count);
		}
	}
}
