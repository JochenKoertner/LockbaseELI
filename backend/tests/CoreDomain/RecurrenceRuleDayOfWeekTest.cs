using System;
using Xunit;
using Lockbase.CoreDomain.Enumerations;
using Lockbase.CoreDomain.ValueObjects;

namespace Lockbase.Tests.CoreDomain
{

	public class RecurrenceRuleDayOfWeekTest {


		[Fact]
		public void TestSimpleDayOfWeek() 
		{
			RecurrenceRuleDayOfWeek rule = "DW";
			Assert.Equal(1, rule.Multiplier); 
			Assert.Equal(TimeInterval.DayOfWeek, rule.Frequency);
			Assert.Equal(7, rule.WeekDays.Count);
		}

        
		[Fact]
		public void TestEveryFriday() 
		{
			RecurrenceRuleDayOfWeek rule = "DW(Fr)";
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
		}

		[Fact]
		public void TestWeekDayValuesPlusSeparated() 
		{
			RecurrenceRuleDayOfWeek rule = "DW(Mo+Tu+We+Th+Fr+Sa+Su)";
			Assert.Equal(7, rule.WeekDays.Count);
		}

		[Fact]
		public void TestWeekDaySetRange() 
		{
			RecurrenceRuleDayOfWeek rule = "DW(Mo-Fr)";
			Assert.Equal(5, rule.WeekDays.Count);
			Assert.DoesNotContain(DayOfWeek.Saturday, rule.WeekDays);
			Assert.DoesNotContain(DayOfWeek.Sunday, rule.WeekDays);
		}

		[Fact]
		public void TestWeekDaySetRangeCaseInsensive()
		{
			RecurrenceRuleDayOfWeek rule = "DW(mo-FR)";
			Assert.Equal(5, rule.WeekDays.Count);
			Assert.DoesNotContain(DayOfWeek.Saturday, rule.WeekDays);
			Assert.DoesNotContain(DayOfWeek.Sunday, rule.WeekDays);
		}

		[Fact]
		public void TestWeekDayValueSetRangeCombined() 
		{
			RecurrenceRuleDayOfWeek rule = "DW(Mo-We+Fr-Su)";
			Assert.Equal(6, rule.WeekDays.Count);
			Assert.DoesNotContain(DayOfWeek.Thursday,rule.WeekDays);
		}
	}
}
