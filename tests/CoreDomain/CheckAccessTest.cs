using System;
using System.Linq;

using Xunit;

using Lockbase.CoreDomain;
using Lockbase.CoreDomain.Entities;
using Lockbase.CoreDomain.Aggregates;
using Lockbase.CoreDomain.ValueObjects;
using System.Globalization;

namespace Lockbase.ui.UnitTest.CoreDomain {

	public class CheckAccessTest {

        private static DateTime ToTime(string timeString) =>
            DateTime.ParseExact(timeString, "yyyy-MM-dd HH:mm", 
                CultureInfo.InvariantCulture);

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
        public void TestNormal(string timeString, string rule, bool expected) 
        {
            TimePeriodDefinition definition = $"20190101T080000Z/28800/{rule}";
			var time = ToTime(timeString);
            var actual = CheckAccess.Check(definition, time);

            if (expected) 
                Assert.True(actual, timeString + " " + rule);
            else
                Assert.False(actual, timeString  + " " + rule); 
        }


        
    }
}