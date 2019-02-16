using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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

        enum Operator {
            Add,
            Range 
        }

        class DayOfWeekAccu {
            
            public ImmutableHashSet<DayOfWeek> Weekdays { get; private set; }
            public Operator Op { get; private set; }

            public DayOfWeekAccu(ImmutableHashSet<DayOfWeek> weekdays, Operator op) {
                this.Weekdays = weekdays;
                this.Op = op;
            }
        }

        private static ISet<DayOfWeek> GetWeekOfDaySet(string values) {
            
            
            if (string.IsNullOrEmpty(values))
                return ImmutableHashSet<DayOfWeek>.Empty;
            
            var seed = new DayOfWeekAccu( ImmutableHashSet<DayOfWeek>.Empty, Operator.Add );

            return Regex.Split(values.Substring(1, values.Length - 2), @"([.+-])")
                .Aggregate(seed, (accu,current) => 
                    {
                        if (current == "+" || current == "-")
                        {
                            return new DayOfWeekAccu( accu.Weekdays, current == "+" ? Operator.Add : Operator.Range );
                        }
                        var operand = WeekOfDayFromAlias(current);

                        if (accu.Op == Operator.Add)
                            return new DayOfWeekAccu( accu.Weekdays.Add(operand), accu.Op);

                        var lastOperand = accu.Weekdays.Last();

                        var range = GetRange(lastOperand, operand);
                            
                        return new DayOfWeekAccu ( range.Aggregate( accu.Weekdays, (a,c) => a.Add(c)), Operator.Add );  
                    })
                .Weekdays;
        }

        private static DayOfWeek WeekOfDayFromAlias(string alias) {
            return Enum.GetValues(typeof(DayOfWeek))
                .Cast<DayOfWeek>()
                .Single( day => alias.Equals(Enum.GetName(typeof(DayOfWeek), day).Substring(0,alias.Length)));
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

        public static IEnumerable<DayOfWeek> GetRange(DayOfWeek start, DayOfWeek end) {
            // In Germany the week start on Monday so we must handle a Range Friday..Sunday correctly
            var totalWeek = Enum.GetValues(typeof(DayOfWeek))
                            .Cast<DayOfWeek>();
            var range = totalWeek
                            .Concat(totalWeek)
                            .SkipWhile( day => day != start)
                            .TakeWhile( day => day != end)
                            .Concat(new []{end});
            return range;
        }

    }
}