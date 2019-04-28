using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions; 

using Lockbase.CoreDomain.Enumerations;

namespace Lockbase.CoreDomain.ValueObjects  {
    
    public class RecurrenceRuleDayOfMonth : RecurrenceRuleDayOfPeriod {

        public static implicit operator RecurrenceRuleDayOfMonth(string value) {

            RecurrenceRule rule = value;
            return rule as RecurrenceRuleDayOfMonth;
        }

        public RecurrenceRuleDayOfMonth(int multiplier, TimeInterval frequency, string values) 
            : base(multiplier, frequency, values, true)
        {
        }
    }
}