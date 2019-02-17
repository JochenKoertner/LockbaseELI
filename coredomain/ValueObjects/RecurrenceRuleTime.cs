using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions; 

using Lockbase.CoreDomain.Enumerations;

namespace Lockbase.CoreDomain.ValueObjects  {
    
    public class RecurrenceRuleTime : RecurrenceRule {

        public static implicit operator RecurrenceRuleTime(string value) {

            RecurrenceRule rule = value;
            return rule as RecurrenceRuleTime;
        }

        public RecurrenceRuleTime(int multiplier, TimeInterval frequency, string values) : base(multiplier, frequency)
        {
            if (frequency == TimeInterval.Second || frequency == TimeInterval.Minute)
                this.Times = GetTimesSet(values, 0, 59);
            else if (frequency == TimeInterval.Hour)
                this.Times = GetTimesSet(values, 0, 23);
            else if (frequency == TimeInterval.WeekOfYear)
                this.Times = GetTimesSet(values, 1, 52);
            else if (frequency == TimeInterval.Month)
                this.Times = GetTimesSet(values, 1, 12);
            else if (frequency == TimeInterval.Year)
                this.Times = GetTimesSet(values, 2000, 2099);
            else throw new ArgumentException(nameof(frequency), frequency.Name);
        }

        public IImmutableSet<int> Times { get; private set; }

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

        private static IImmutableSet<int> GetTimesSet(string values, int start, int end) {
            
            if (string.IsNullOrEmpty(values))
                return AddRange<int>(ImmutableHashSet<int>.Empty, Enumerable.Range(start, end-start+1));
            
            return ImmutableHashSet<int>.Empty;
        }

        private static IImmutableSet<T> AddRange<T>(IImmutableSet<T> immutableSet, IEnumerable<T> range) 
            => range.Aggregate(immutableSet, (accu, current) => accu.Add(current));

        private static IEnumerable<T> GetAllEnums<T>()  {
            return Enum.GetValues(typeof(T))
                            .Cast<T>();
        }
    }
}