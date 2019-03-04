using System;
using Xunit; 

using Lockbase.CoreDomain.Entities;
using Lockbase.CoreDomain.Enumerations;
using Lockbase.CoreDomain.ValueObjects;
using System.Linq;

namespace Lockbase.ui.UnitTest.CoreDomain {

	public class RecurrenceRuleTimeTest {

		[Theory]
		[InlineData("s", 60)]
		[InlineData("m", 60)]
		[InlineData("h", 24)]
		[InlineData("WY", 52)]
		[InlineData("M", 12)]
		[InlineData("DM", 31)]
		[InlineData("Y", 100)]
		public void TestSimpleRule(string alias, int count) 
		{
			RecurrenceRuleTime rule = alias;
			Assert.Equal(1, rule.Multiplier); 
			Assert.Equal(alias, rule.Frequency.Alias);
			Assert.Equal(count, rule.Times.Count);
		}

		[Fact]
		public void TestYearRange() 
		{
			RecurrenceRuleTime rule = "Y(2012-2014)";
			Assert.Equal(TimeInterval.Year, rule.Frequency);
			Assert.Contains(2012, rule.Times);
			Assert.Contains(2013, rule.Times);
			Assert.Contains(2014, rule.Times);
			Assert.Equal(3, rule.Times.Count);
		}

		[Fact]
		public void TestMonthSingle()
		{
			RecurrenceRuleTime rule = "M(1)";
			Assert.Equal(TimeInterval.Month, rule.Frequency);
			Assert.Contains(1, rule.Times);
			Assert.Single(rule.Times);
		}

		[Fact]
		public void TestWeekSet()
		{
			RecurrenceRuleTime rule = "WY(23+35)";
			Assert.Equal(TimeInterval.WeekOfYear, rule.Frequency);
			Assert.Contains(23, rule.Times);
			Assert.Contains(35, rule.Times);
			Assert.Equal(2,rule.Times.Count);
		}

		[Fact]
		public void TestDayOfMonth()
		{
			RecurrenceRuleTime rule = "DM(5+15)";
			Assert.Equal(TimeInterval.DayOfMonth, rule.Frequency);
			Assert.Contains(5, rule.Times);
			Assert.Contains(15, rule.Times);
			Assert.Equal(2,rule.Times.Count);
		}
	}
}
