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
                
                case RecurrenceRuleDayOfMonth dayOfMonthRule: 
                    return dayOfMonthRule.WeekDays
                        .SelectMany(  spec => AddDates(time.Year, time.Month, spec))
                        .Contains(time.Date);

                case RecurrenceRuleTime timesRule:
                    if (timesRule.Frequency == TimeInterval.DayOfMonth)
                        return timesRule.Times.Contains(time.Day);
                    
                    if (timesRule.Frequency == TimeInterval.DayOfYear)
                        return timesRule.Times.Contains(time.DayOfYear);

                    if (timesRule.Frequency == TimeInterval.Month)
                        return timesRule.Times.Contains(time.Month);

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