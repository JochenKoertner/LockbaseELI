using System;
using Xunit; 

using Lockbase.CoreDomain.ValueObjects;
using System.Globalization;
using Lockbase.CoreDomain.Enumerations;

namespace Lockbase.ui.UnitTest.CoreDomain {

	public class TimePeriodDefinitionTest {

		[Fact]
		public void TimePeriodDefinitionAssignmentFull() {
			TimePeriodDefinition definition = "20190211T080000Z/28800/DW(Mo+Tu+We+Th+Fr)/20190329T160000Z";
			
			Assert.Equal(28800, definition.Duration); 
			Assert.Equal(new DateTime(2019,02,11,8,0,0), definition.StartTime);
			Assert.Equal(new DateTime(2019,03,29,16,0,0), definition.EndTime);
			Assert.Equal(TimeInterval.DayOfWeek, definition.RecurrenceRule.Frequency);
		}

		[Fact]
		public void TimePeriodDefinitionAssignmentStartEnd() {
			TimePeriodDefinition definition = "20190211T080000Z///20190329T160000Z";
			
			Assert.Null(definition.Duration); 
			Assert.Equal(new DateTime(2019,02,11,8,0,0), definition.StartTime);
			Assert.Equal(new DateTime(2019,03,29,16,0,0), definition.EndTime);
			Assert.Null(definition.RecurrenceRule);
		}

		[Fact]
		public void TimePeriodDefinitionAssignmentDurationRecurence() {
			TimePeriodDefinition definition = "/28800/DW";
			
			Assert.Equal(28800, definition.Duration); 
			Assert.Null(definition.StartTime);
			Assert.Null(definition.EndTime);
			Assert.Equal(TimeInterval.DayOfWeek, definition.RecurrenceRule.Frequency);
		}

		[Fact]
		public void TimePeriodDefinitionAssignmentNothing() {
			TimePeriodDefinition definition = "";
			
			Assert.Null(definition.Duration); 
			Assert.Null(definition.StartTime);
			Assert.Null(definition.EndTime);
			Assert.Null(definition.RecurrenceRule);
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
