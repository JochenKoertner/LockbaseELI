using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions; 

using Lockbase.CoreDomain.Enumerations;

namespace Lockbase.CoreDomain.ValueObjects  {


    // seealos  RFC 5545 !! 
    // https://regex101.com/r/vln7Wv/1/

    public class RecurrenceRule {


        /// implizite Konvertierung vom String
        public static implicit operator RecurrenceRule(string value)
        {
            const string pattern = @"(?'multiplier'\d*)(?'frequency'DWY|DWM|DW|WY|M|Y)(?'values'\(.+\))*";

            RegexOptions options = RegexOptions.IgnoreCase;

            Match match = Regex.Matches(value, pattern, options).First();


            Group multiplierGroup = match.Groups["multiplier"];
            Group frequencyGroup = match.Groups["frequency"];
            Group valuesGroup = match.Groups["values"];
            
            int multiplier = String.IsNullOrEmpty(multiplierGroup.Value) ? 1 : Convert.ToInt32(multiplierGroup.Value);
            TimeInterval frequency = TimeInterval.GetAll().SingleOrDefault( ti => ti.Alias == frequencyGroup.Value);

            string values = valuesGroup.Value;
            
            return new RecurrenceRule(multiplier, frequency, GetWeekOfDaySet(values) );
        }

        private static ISet<DayOfWeek> GetWeekOfDaySet(string values) {
             
            var weekDays = values.Substring(1, values.Length - 2)
                .Split(',')
                .ToList<string>()
                .Select( WeekOfDayFromAlias );

            return new HashSet<DayOfWeek>(weekDays);
        }

        private static DayOfWeek WeekOfDayFromAlias(string alias) {
            if (alias.Substring(0,2).Equals("Mo", StringComparison.OrdinalIgnoreCase)) 
                return DayOfWeek.Monday;
            else if (alias.Substring(0,2).Equals("Tu", StringComparison.OrdinalIgnoreCase))
                return DayOfWeek.Tuesday;
            else if(alias.Substring(0,2).Equals("We", StringComparison.OrdinalIgnoreCase))
                return DayOfWeek.Wednesday;
            else if(alias.Substring(0,2).Equals("Th", StringComparison.OrdinalIgnoreCase))
                return DayOfWeek.Thursday;
            else if(alias.Substring(0,2).Equals("Fr", StringComparison.OrdinalIgnoreCase))
                return DayOfWeek.Friday;
            else if(alias.Substring(0,2).Equals("Sa", StringComparison.OrdinalIgnoreCase))
                return DayOfWeek.Saturday;
            else if(alias.Substring(0,2).Equals("Su", StringComparison.OrdinalIgnoreCase))
                return DayOfWeek.Sunday;
            throw new ArgumentException(nameof(alias), alias);
            
        }

        public RecurrenceRule(int multiplier, TimeInterval frequency, ISet<DayOfWeek> weekdays) 
        {
            this.Multiplier = multiplier;
            this.Frequency = frequency;
            this.WeekDays = new HashSet<DayOfWeek>(weekdays);
        }

        public int Multiplier { get; private set; }

        public TimeInterval Frequency { get; private set; }

        public HashSet<DayOfWeek> WeekDays { get; private set; }

    }
}