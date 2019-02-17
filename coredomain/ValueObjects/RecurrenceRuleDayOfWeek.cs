using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions; 

using Lockbase.CoreDomain.Enumerations;

namespace Lockbase.CoreDomain.ValueObjects  {
    
    public class RecurrenceRuleDayOfWeek : RecurrenceRule {

        public static implicit operator RecurrenceRuleDayOfWeek(string value) {

            RecurrenceRule rule = value;
            return rule as RecurrenceRuleDayOfWeek;
        }

        public RecurrenceRuleDayOfWeek(int multiplier, TimeInterval frequency, string values) : base(multiplier, frequency)
        {
            this.WeekDays = GetWeekOfDaySet(values);
        }

        public IImmutableSet<DayOfWeek> WeekDays { get; private set; }

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

        private static IImmutableSet<DayOfWeek> GetWeekOfDaySet(string values) {
            
            if (string.IsNullOrEmpty(values))
                return AddRange<DayOfWeek>(ImmutableHashSet<DayOfWeek>.Empty, GetAllEnums<DayOfWeek>());
            
            var seed = new DayOfWeekAccu( ImmutableHashSet<DayOfWeek>.Empty, Operator.Add );

            // https://regex101.com/r/4NRHND/2
            RegexOptions options = RegexOptions.IgnoreCase;
            string input = values.Substring(1, values.Length - 2);
            string pattern = @"([+\-,])|(Mo|Tu|We|Th|Fr|Sa|Su)";
            
            return Regex.Matches(input, pattern, options)
                .Select(  match => match.Value )
                .Aggregate(seed, (accu,current) => 
                    {
                        if (current == "+" || current == "-" || current == ",")
                        {
                            return new DayOfWeekAccu( accu.Weekdays, (current == "+" || current == ",") ? Operator.Add : Operator.Range );
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
                .Single( day => alias.Equals(Enum.GetName(typeof(DayOfWeek), day).Substring(0,alias.Length),
                    StringComparison.OrdinalIgnoreCase));
        }


        private static IEnumerable<DayOfWeek> GetRange(DayOfWeek start, DayOfWeek end) {
            // In Germany the week start on Monday so we must handle a Range Friday..Sunday correctly
            var totalWeek = GetAllEnums<DayOfWeek>();
            var range = totalWeek
                            .Concat(totalWeek)
                            .SkipWhile( day => day != start)
                            .TakeWhile( day => day != end)
                            .Concat(new []{end});
            return range;
        }

        private static IImmutableSet<T> AddRange<T>(IImmutableSet<T> immutableSet, IEnumerable<T> range) 
            => range.Aggregate(immutableSet, (accu, current) => accu.Add(current));

        private static IEnumerable<T> GetAllEnums<T>()  {
            return Enum.GetValues(typeof(T))
                            .Cast<T>();
        }
    }
}