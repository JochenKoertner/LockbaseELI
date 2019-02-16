using System;
using Xunit; 

using Lockbase.CoreDomain.Entities;
using Lockbase.CoreDomain.Enumerations;
using Lockbase.CoreDomain.ValueObjects;
using System.Linq;

namespace Lockbase.ui.UnitTest.CoreDomain {

	public class RecurrenceRuleTest {

        
		[Fact]
		public void TestEveryFriday() 
		{
			RecurrenceRule rule = "DW(Fr)";
			Assert.Equal(1, rule.Multiplier); 
			Assert.Equal(TimeInterval.DayOfWeek, rule.Frequency);
			Assert.Contains(DayOfWeek.Friday, rule.WeekDays);
			Assert.Single(rule.WeekDays);
		}

		[Fact]
		public void TestMultiplier() 
		{
			RecurrenceRule rule = "42DW";
			Assert.Equal(42, rule.Multiplier); 
			Assert.Equal(TimeInterval.DayOfWeek, rule.Frequency);
			Assert.Empty(rule.WeekDays);
		}

		[Fact]
		public void TestWeekDayValues() 
		{
			RecurrenceRule rule = "DW(Mo+Tu+We+Th+Fr+Sa+Su)";
			Assert.Equal(7, rule.WeekDays.Count);
		}

		[Fact]
		public void TestWeekDayValueRange() 
		{
			RecurrenceRule rule = "DW(Mo-We+Fr-Su)";
			Assert.Equal(6, rule.WeekDays.Count);
			Assert.False(rule.WeekDays.Contains(DayOfWeek.Thursday));
		}

		[Fact]
		public void TestDayOfWeekRange() {

			var range = RecurrenceRule.GetRange(DayOfWeek.Monday, DayOfWeek.Wednesday);
			                
			Assert.Equal(3, range.Count());
		}

		[Fact]
		public void TestDayOfWeekRangeCircle() {

			var range = RecurrenceRule.GetRange(DayOfWeek.Friday, DayOfWeek.Sunday);
			                
			Assert.Equal(3, range.Count());
		}
	}
}
