using System;
using Xunit; 

using Lockbase.CoreDomain.ValueObjects;
using System.Globalization;
using Lockbase.CoreDomain.Enumerations;
using System.Linq;

namespace Lockbase.ui.UnitTest.CoreDomain {

	public class TimePeriodDefinitionTest {

		[Fact]
		public void TimePeriodDefinitionAssignmentFull() {
			TimePeriodDefinition definition = "20190211T080000Z/28800/DW(Mo+Tu+We+Th+Fr)/20190329T160000Z";
			
			Assert.Equal(28800, definition.Duration); 
			Assert.Equal(new DateTime(2019,02,11,8,0,0), definition.StartTime);
			Assert.Equal(new DateTime(2019,03,29,16,0,0), definition.EndTime);
			Assert.Equal(TimeInterval.DayOfWeek, definition.RecurrenceRules.First().Frequency);
		}


		[Fact]
		public void TimePeriodDefinitionAssignmentMultiRecurence() {
			TimePeriodDefinition definition = "20190211T080000Z/28800/DWM(Su1+Su3);M(1)/20190329T160000Z";
			
			Assert.NotNull(definition.Duration); 
			Assert.NotNull(definition.StartTime);
			Assert.NotNull(definition.EndTime);
			Assert.Equal(TimeInterval.DayOfWeekPerMonth, definition.RecurrenceRules.First().Frequency);
			Assert.Equal(TimeInterval.Month, definition.RecurrenceRules.Last().Frequency);
		}


		[Fact]
		public void TimePeriodDefinitionAssignmentNothing() {
			TimePeriodDefinition definition = "";
			
			Assert.Null(definition.Duration); 
			Assert.Null(definition.StartTime);
			Assert.Null(definition.EndTime);
			Assert.Empty(definition.RecurrenceRules);
		}

		[Theory]
		[InlineData("//DW")]
		[InlineData("///20190329T160000Z")]
		[InlineData("20190211T080000Z//DW")]
		[InlineData("20190211T080000Z///20190329T160000Z")]
		[InlineData("/28800/DW")]
		[InlineData("/28800//20190329T160000Z")]
		public void TimePeriodDefinitionInvalid(string invalid) {
			// Wiederholung und Endedatum setzen Startzeit und Dauer vorraus
			Assert.Throws<ArgumentNullException>( () => { TimePeriodDefinition definition = invalid;} );
		}

		[Theory]
		[InlineData("20190211T080000Z/28800/DW(Mo,Tu,We,Th,Fr)/20190329T160000Z")]
		[InlineData("20190401T070000Z/28800/DW(Mo,Tu,We,Th,Fr)/20191025T150000Z")]
		[InlineData("20191028T080000Z/28800/DW(Mo,Tu,We,Th,Fr)/20200210T160000Z")]
		[InlineData("20190211T070000Z/28800/DW(Mo,Tu,We,Th,Fr)/20190329T150000Z")]
		[InlineData("20190401T060000Z/28800/DW(Mo,Tu,We,Th,Fr)/20191025T140000Z")]
		[InlineData("20191028T070000Z/28800/DW(Mo,Tu,We,Th,Fr)/20200211T150000Z")]
		[InlineData("20190211T050000Z/54000/DW/20190330T200000Z")]
		[InlineData("20190331T040000Z/54000/DW/20191026T190000Z")]
		[InlineData("20191027T050000Z/54000/DW/20200211T200000Z")]
		[InlineData("20181231T230000Z/63072000")]
		[InlineData("20190211T200000Z/32400/DW/20190330T050000Z")]
		[InlineData("20190330T200000Z/28800")]
		[InlineData("20190331T190000Z/32400/DW/20191026T040000Z")]
		[InlineData("20191026T190000Z/36000")]
		[InlineData("20191027T200000Z/32400/DW/20200211T050000Z")]
		public void TimePeriodIsValid(string value) {
			TimePeriodDefinition definition = value;
			Assert.NotNull(definition);
		}

		[Fact]
		public void StringToDateTimeTest() 
		{
			Assert.Equal(new DateTime(2019,02,11,8,0,0),
				TimePeriodDefinition.StringToDateTime("20190211T080000Z"));
			Assert.Equal(new DateTime(2019,03,29,16,0,0),
				TimePeriodDefinition.StringToDateTime("20190329T160000Z"));
		}
	}
}
