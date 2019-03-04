using System;
using System.Linq;
using Xunit; 

using Lockbase.CoreDomain.Entities;
using Lockbase.CoreDomain.Enumerations;
using Lockbase.CoreDomain.ValueObjects;

namespace Lockbase.ui.UnitTest.CoreDomain {

	public class RecurrenceRuleDayOfMonthTest {


		[Fact]
		public void TestFirstDayOfMonth() 
		{
			RecurrenceRuleDayOfMonth rule = "DWM(Mo1)";
			Assert.Equal(1, rule.Multiplier); 
			Assert.Equal(TimeInterval.DayOfWeekPerMonth, rule.Frequency);
			Assert.Single(rule.WeekDays);

			Assert.Contains(new DayOfWeekSpecified(DayOfWeek.Monday, +1), rule.WeekDays);
		}

		[Fact]
		public void Test2ndSundayLastMonday() 
		{
			RecurrenceRuleDayOfMonth rule = "DWM(Su2+Mo-1)";
			Assert.Equal(1, rule.Multiplier); 
			Assert.Equal(TimeInterval.DayOfWeekPerMonth, rule.Frequency);
			Assert.Equal(2, rule.WeekDays.Count);

			Assert.Contains(new DayOfWeekSpecified(DayOfWeek.Sunday, +2), rule.WeekDays);
			Assert.Contains(new DayOfWeekSpecified(DayOfWeek.Monday, -1), rule.WeekDays);
		}
	}
}
