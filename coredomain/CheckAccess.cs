using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Lockbase.CoreDomain.ValueObjects;

namespace Lockbase.CoreDomain {

    public static class CheckAccess {
        
        public static bool Check(IEnumerable<TimePeriodDefinition> timeDefinitions, DateTime time) {
            var definition = timeDefinitions.SingleOrDefault(
                td => Check(td, time));  

            return (definition != null);
        }

        public static bool Check(TimePeriodDefinition td, DateTime time) {

            if ((td.StartTime.HasValue && td.StartTime > time) 
                || (td.EndTime.HasValue && time > td.EndTime))
                return false;

            if (td.Duration.HasValue && td.StartTime.HasValue)
            {
                TimeSpan start = td.StartTime.Value.TimeOfDay;
                var allowed = td.Duration.Value;
                var current = time.TimeOfDay.Subtract(start).TotalSeconds;
                if (current > allowed || current < 0)
                    return false;
            }

            return td.RecurrenceRules.Aggregate(true, 
                (accu,current) => accu && CheckRecurrenceRule(current, time));
        } 

        private static bool CheckRecurrenceRule(RecurrenceRule rule, DateTime time) 
        {
            switch (rule)
            {
                case RecurrenceRuleDayOfWeek dayOfWeekRule: 
                    return dayOfWeekRule.WeekDays.Contains(time.DayOfWeek);
                
          /*     case RecurrenceRuleDayOfMonth dayOfMonthRule: 
                    var dayOfMonthRule.WeekDays.SelectMany(  spec => AddDates(time.Year, time.Month, spec));
                    return dayOfMonthRule.WeekDays.Contains(time.DayOfWeek);
             */   
                default:
                    return true;
            }
        }

        private static IEnumerable<DateTime> AddDates(int year, int month, DayOfWeekSpecified specified)
        {
            var weekdates = Enumerable
                .Range(1, DateTime.DaysInMonth(year, month))
                .Select( day => new DateTime(year,month,day))
                .Where( d => d.DayOfWeek == specified.DayOfWeek);

            DateTime date = default(DateTime);
            if (specified.Specifier > 0)
                date = weekdates.ElementAtOrDefault(specified.Specifier-1);
            else if (specified.Specifier < 0)
                date = weekdates.ElementAtOrDefault(weekdates.Count() + specified.Specifier);

            if (date != default(DateTime))
                yield return date;
        } 
    }
}