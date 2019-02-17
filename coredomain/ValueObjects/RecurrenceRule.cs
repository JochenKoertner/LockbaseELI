using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions; 

using Lockbase.CoreDomain.Enumerations;

namespace Lockbase.CoreDomain.ValueObjects  {


    // see also  RFC 5545 !! 
    
    public class RecurrenceRule {


        /// implizite Konvertierung vom String
        public static implicit operator RecurrenceRule(string value)
        {
            // https://regex101.com/r/vln7Wv/2/

            const string pattern = @"(?'multiplier'\d*)(?'frequency's|m|h|DWY|DWM|DW|WY|M|Y)(?'values'\(.+\))*";

            RegexOptions options = RegexOptions.IgnoreCase;

            Match match = Regex.Matches(value, pattern, options).First();


            Group multiplierGroup = match.Groups["multiplier"];
            Group frequencyGroup = match.Groups["frequency"];
            Group valuesGroup = match.Groups["values"];
            
            int multiplier = String.IsNullOrEmpty(multiplierGroup.Value) ? 1 : Convert.ToInt32(multiplierGroup.Value);
            TimeInterval frequency = TimeInterval.GetAll().SingleOrDefault( ti => ti.Alias == frequencyGroup.Value);

            string values = valuesGroup.Value;

            if (frequency == TimeInterval.DayOfWeek)
                return new RecurrenceRuleDayOfWeek(multiplier, frequency, values);
            else if (frequency.IsTimesRange)
                return new RecurrenceRuleTime(multiplier, frequency, values);

            return new RecurrenceRule(multiplier, frequency);
        }

        public RecurrenceRule(int multiplier, TimeInterval frequency) 
        {
            this.Multiplier = multiplier;
            this.Frequency = frequency;
        }


        public int Multiplier { get; private set; }

        public TimeInterval Frequency { get; private set; }

        protected static IEnumerable<T> GetAllEnums<T>()  {
            return Enum.GetValues(typeof(T))
                            .Cast<T>();
        }

        protected enum Operator {
            Add,
            Range 
        }

        protected class ReductionState<T> {
            
            public IImmutableSet<T> Reduction { get; private set; }
            public Operator Op { get; private set; }

            public ReductionState(IImmutableSet<T> reduction, Operator op) {
                this.Reduction = reduction;
                this.Op = op;
            }

            public ReductionState() {
                this.Reduction = ImmutableHashSet<T>.Empty;
                this.Op = Operator.Add;
            }
        }
    }
}