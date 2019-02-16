using System;
using Xunit; 

using Lockbase.CoreDomain.Entities;
using Lockbase.CoreDomain.Enumerations;
using Lockbase.CoreDomain.ValueObjects;

namespace Lockbase.ui.UnitTest.CoreDomain {

	public class RecurrenceRuleTest {

        
		[Fact]
		public void TestEveryFriday() 
		{
			RecurrenceRule rule = "DW(Fr)";
			Assert.Equal(rule.Multiplier, 1); 
			Assert.Equal(rule.Frequency, TimeInterval.DayOfWeek);
			Assert.True(rule.WeekDays.Contains(DayOfWeek.Friday));
			Assert.Equal(rule.WeekDays.Count, 1);
		}
	}
}
