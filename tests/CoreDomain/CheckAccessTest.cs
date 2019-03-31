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
    }
}