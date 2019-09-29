using System;
using Xunit;
using Lockbase.CoreDomain.Enumerations;
using Lockbase.CoreDomain.ValueObjects;

namespace Lockbase.Tests.CoreDomain
{

	public class RecurrenceRuleDayOfYearTest {


		[Fact]
		public void TestFirstDayOfYear() 
		{
			RecurrenceRuleDayOfYear rule = "DWY(Mo50)";
			Assert.Equal(1, rule.Multiplier); 
			Assert.Equal(TimeInterval.DayOfWeekPerYear, rule.Frequency);
			Assert.Single(rule.WeekDays);

			Assert.Contains(new DayOfWeekSpecified(DayOfWeek.Monday, +50), rule.WeekDays);
		}

		[Fact]
		public void TestLastSunday() 
		{
			RecurrenceRuleDayOfYear rule = "DWY(Su-1)";
			Assert.Equal(1, rule.Multiplier); 
			Assert.Equal(TimeInterval.DayOfWeekPerYear, rule.Frequency);
			Assert.Single(rule.WeekDays);

			Assert.Contains(new DayOfWeekSpecified(DayOfWeek.Sunday, -1), rule.WeekDays);
		}
	}
}
