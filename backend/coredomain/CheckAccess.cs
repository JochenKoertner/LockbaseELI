using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Lockbase.CoreDomain.Enumerations;
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
					var seconds = td.Duration.Value;

					if (seconds >= new TimeSpan(24,0,0).TotalSeconds) {
						var endTime = td.StartTime.Value.AddSeconds(seconds);
						if (time < td.StartTime || time > endTime)
							return false;
					} 
					else {
						TimeSpan start = td.StartTime.Value.TimeOfDay;
						var current = time.TimeOfDay.Subtract(start).TotalSeconds;
						if (current > seconds || current < 0)
							return false;
					}
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
                
                case RecurrenceRuleDayOfMonth dayOfMonthRule: 
                    return dayOfMonthRule.WeekDays
                        .SelectMany(  spec => AddDates(time.Year, time.Month, spec))
                        .Contains(time.Date);

                case RecurrenceRuleTime timesRule:
                    if (timesRule.Frequency == TimeInterval.DayOfMonth)
                        return CheckDayOfMonth(time, timesRule.Times);
                    
                    if (timesRule.Frequency == TimeInterval.DayOfYear)
                        return CheckDayOfYear(time, timesRule.Times);

                    if (timesRule.Frequency == TimeInterval.Month)
                        return CheckMonthMultiplier(time, timesRule.Multiplier)
                            && timesRule.Times.Contains(time.Month);

                    if (timesRule.Frequency == TimeInterval.Year)
                        return timesRule.Times.Contains(time.Year);

                    if (timesRule.Frequency == TimeInterval.Hour)
                        return timesRule.Times.Contains(time.Hour);

                    if (timesRule.Frequency == TimeInterval.Minute)
                        return timesRule.Times.Contains(time.Minute);

                    if (timesRule.Frequency == TimeInterval.Second)
                        return timesRule.Times.Contains(time.Second);

                    if (timesRule.Frequency == TimeInterval.WeekOfYear)
                        return timesRule.Times.Contains(time.WeekOfYear());

                    return false;
                
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

        private static bool CheckMonthMultiplier(DateTime time, int multiplier) {
            if (multiplier == 1)
                return true;
            else 
                return false;
        }
        
        private static bool CheckDayOfMonth(DateTime time, IImmutableSet<int> times) {
            return times.Aggregate(seed: false, 
                func: (accu,current) => {
                    if (current > 0)
                        return accu || time.Day == current;
                    else 
                        return accu || time.Day == new DateTime(time.Year, time.Month, 1).AddMonths(1).AddDays(current).Day;
                });
        }

         private static bool CheckDayOfYear(DateTime time, IImmutableSet<int> times) {
            return times.Aggregate(seed: false, 
                func: (accu,current) => {
                    if (current > 0)
                        return accu || time.DayOfYear == current;
                    else 
                        return accu || time.DayOfYear == new DateTime(time.Year, 12, 31).AddDays(current+1).DayOfYear;
                });
        }
        private static int WeekOfYear(this DateTime date) {

            CultureInfo currentCulture = CultureInfo.CurrentCulture;

            // Aktuellen Kalender ermitteln
            Calendar calendar = currentCulture.Calendar;

            // Kalenderwoche über das Calendar-Objekt ermitteln
            int calendarWeek = calendar.GetWeekOfYear(date,
               currentCulture.DateTimeFormat.CalendarWeekRule,
               currentCulture.DateTimeFormat.FirstDayOfWeek);

            // Überprüfen, ob eine Kalenderwoche größer als 52
            // ermittelt wurde und ob die Kalenderwoche des Datums
            // in einer Woche 2 ergibt: In diesem Fall hat
            // GetWeekOfYear die Kalenderwoche nicht nach ISO 8601 
            // berechnet (Montag, der 31.12.2007 wird z. B.
            // fälschlicherweise als KW 53 berechnet). 
            // Die Kalenderwoche wird dann auf 1 gesetzt
            if (calendarWeek > 52)
            {
                date = date.AddDays(7);
                int testCalendarWeek = calendar.GetWeekOfYear(date,
                   currentCulture.DateTimeFormat.CalendarWeekRule,
                   currentCulture.DateTimeFormat.FirstDayOfWeek);
                if (testCalendarWeek == 2)
                    calendarWeek = 1;
            }

            return calendarWeek;
        }
    }
}