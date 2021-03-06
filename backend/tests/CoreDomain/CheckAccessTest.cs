using System;
using System.Globalization;

using Xunit;

using Lockbase.CoreDomain;
using Lockbase.CoreDomain.ValueObjects;

namespace Lockbase.Tests.CoreDomain
{

	public class CheckAccessTest {

		private static DateTime ToTime(string timeString) =>
			DateTime.ParseExact(timeString, "yyyy-MM-dd HH:mm", 
				CultureInfo.InvariantCulture);

			[Theory]
			[InlineData("2018-12-30 12:00", false)]
			[InlineData("2019-12-30 12:00", true)]
			[InlineData("2020-12-30 12:00", true)]
			[InlineData("2021-12-30 12:00", false)]
			public void TestWithoutRecurrencyRule(string timeString, bool expected)
			{
				TimePeriodDefinition definition = "20181231T230000Z/63072000";
				var time = ToTime(timeString);
				var actual = CheckAccess.Check(definition, time);

				if (expected) 
						Assert.True(actual, timeString);
				else
						Assert.False(actual, timeString);
			}

		[Theory]
		// Freitags
		[InlineData("2019-02-15 12:00", true)]
		[InlineData("2019-02-15 16:30", false)]
		[InlineData("2019-02-15 07:30", false)]
	   
		// Nicht im Range
		[InlineData("2018-02-15 12:00", false)]
		[InlineData("2020-02-15 12:00", false)]

		// Samstag & Sonntag
		[InlineData("2019-02-16 12:00", false)]
		[InlineData("2019-02-17 12:00", false)]
   
		public void TestWorkingTime(string timeString, bool expected) 
		{
			TimePeriodDefinition definition = "20190211T080000Z/28800/DW(Mo+Tu+We+Th+Fr)/20190329T160000Z";
			var time = ToTime(timeString);
			var actual = CheckAccess.Check(definition, time);

			if (expected) 
				Assert.True(actual, timeString);
			else
				Assert.False(actual, timeString); 
		}

		[Theory]
		[InlineData("2019-01-13 12:00", true)]  // 2nd Sunday
		[InlineData("2019-01-28 12:00", true)]  // Last Monday
		[InlineData("2019-01-06 12:00", false)] // 1st Sunday
		[InlineData("2019-01-21 12:00", false)] // Some Monday
		public void TestDayWeekOfMonth(string timeString, bool expected) 
		{
			// "2. Sonntag im Monat und Letzter Montag" 
			TimePeriodDefinition definition = "20190101T080000Z/28800/DWM(Su2+Mo-1)";
			var time = ToTime(timeString);
			var actual = CheckAccess.Check(definition, time);

			if (expected) 
				Assert.True(actual, timeString);
			else
				Assert.False(actual, timeString); 
		}

		[Theory]
		[InlineData("2019-01-01 12:00", true)]  
		[InlineData("2019-02-15 12:00", true)]  
		[InlineData("2019-01-02 12:00", false)] 
		[InlineData("2019-02-16 12:00", false)] 
		public void TestDayOfMonth(string timeString, bool expected) 
		{
			// "1. und 15. im Monat" 
			TimePeriodDefinition definition = "20190101T080000Z/28800/DM(1+15)";
			var time = ToTime(timeString);
			var actual = CheckAccess.Check(definition, time);

			if (expected) 
				Assert.True(actual, timeString);
			else
				Assert.False(actual, timeString); 
		}

		[Theory]
		[InlineData("2019-01-31 12:00", true)]  
		[InlineData("2019-02-28 12:00", true)]  
		[InlineData("2019-01-30 12:00", false)] 
		[InlineData("2019-02-27 12:00", false)] 
		public void TestLastDayOfMonth(string timeString, bool expected) 
		{
			// "der Monatsletzte" 
			TimePeriodDefinition definition = "20190101T080000Z/28800/DM(-1)";
			var time = ToTime(timeString);
			var actual = CheckAccess.Check(definition, time);

			if (expected) 
				Assert.True(actual, timeString);
			else
				Assert.False(actual, timeString); 
		}
		[Theory]
		[InlineData("2019-12-30 12:00", true)]  
		[InlineData("2020-12-30 12:00", true)]  
		[InlineData("2019-12-31 12:00", false)] 
		[InlineData("2019-12-29 12:00", false)] 
		public void TestLastDayOfYear(string timeString, bool expected) 
		{
			// "der Monatsletzte" 
			TimePeriodDefinition definition = "20190101T080000Z/28800/DY(-2)";
			var time = ToTime(timeString);
			var actual = CheckAccess.Check(definition, time);

			if (expected) 
				Assert.True(actual, timeString);
			else
				Assert.False(actual, timeString); 
		}

		[Theory]
		[InlineData("2019-02-09 12:00", true)]  // 40. 
		[InlineData("2019-02-10 12:00", true)]  // 41. 
		[InlineData("2019-02-08 12:00", false)] // 39.
		[InlineData("2019-02-11 12:00", false)] // 42. 
		public void TestDayOfYear(string timeString, bool expected) 
		{
			// "1. und 15. im Monat" 
			TimePeriodDefinition definition = "20190101T080000Z/28800/DY(40+41)";
			var time = ToTime(timeString);
			var actual = CheckAccess.Check(definition, time);

			if (expected) 
				Assert.True(actual, timeString);
			else
				Assert.False(actual, timeString); 
		
		}
		[Theory]
		[InlineData("2019-02-09 12:00", "M(2)", true)]  // Februar 
		[InlineData("2019-03-09 12:00", "M(2)", false)]  // Februar 
		[InlineData("2019-02-09 12:00", "Y(2018-2020)", true)]  // 2019 
		[InlineData("2017-02-09 12:00", "Y(2018-2020)", false)]  // 2017 
		[InlineData("2021-02-09 12:00", "Y(2018-2020)", false)]  // 2021 
		[InlineData("2019-02-09 12:00", "h(12)", true)]      // 12:00
		[InlineData("2019-02-09 13:00", "h(12)", false)]     // 13:00
		[InlineData("2019-02-09 12:17", "m(15-20)", true)]      // 12:17
		[InlineData("2019-02-09 12:15", "m(15-20)", true)]      // 12:15
		[InlineData("2019-02-09 12:20", "m(15-20)", true)]      // 12:20
		[InlineData("2019-02-09 12:14", "m(15-20)", false)]     // 12:14
		[InlineData("2019-02-09 12:21", "m(15-20)", false)]     // 12:14
		[InlineData("2019-06-03 12:00", "WY(23+35)", true)]     // 23. KW
		[InlineData("2019-08-26 12:00", "WY(23+35)", true)]     // 35. KW
		[InlineData("2019-05-27 12:00", "WY(23+35)", false)]    // 22. KW
		[InlineData("2019-09-02 12:00", "WY(23+35)", false)]    // 36. KW
		public void TestNormal(string timeString, string rule, bool expected) 
		{
			TimePeriodDefinition definition = $"20100101T080000Z/28800/{rule}";
			var time = ToTime(timeString);
			var actual = CheckAccess.Check(definition, time);

			if (expected) 
				Assert.True(actual, timeString + " " + rule);
			else
				Assert.False(actual, timeString  + " " + rule); 
		}   

		[Theory]
		[InlineData("2019-02-04 12:00", true)]  // Montag Feb
		[InlineData("2019-02-11 12:00", true)]  // Montag Feb
		[InlineData("2019-02-05 12:00", false)] // Dienstag Februar
		[InlineData("2019-01-21 12:00", false)] // Montag Januar
		public void TestCombinedRule(string timeString, bool expected) 
		{
			// "Jeden Montag im Febuar" 
			TimePeriodDefinition definition = "20190101T080000Z/28800/DW(Mo);M(2)";
			var time = ToTime(timeString);
			var actual = CheckAccess.Check(definition, time);

			if (expected) 
				Assert.True(actual, timeString);
			else
				Assert.False(actual, timeString); 
		
		}     

/* 
		[Theory]
		[InlineData("2019-01-01 12:00", true)]  // Jan
		[InlineData("2019-02-01 12:00", false)]  // Feb
		[InlineData("2019-03-01 12:00", false)] // Mär
		[InlineData("2019-04-01 12:00", true)]  // Apr 
		public void TestMultiplierRule(string timeString, bool expected) 
		{
			// "Alle drei Monate" 
			TimePeriodDefinition definition = "20190101T080000Z/28800/3M";
			var time = ToTime(timeString);
			var actual = CheckAccess.Check(definition, time);

			if (expected) 
				Assert.True(actual, timeString);
			else
				Assert.False(actual, timeString); 
		
		}     
		*/
	}
}