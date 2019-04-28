using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions; 

using Lockbase.CoreDomain.Enumerations;

namespace Lockbase.CoreDomain.ValueObjects  {
    
    public class RecurrenceRuleDayOfYear : RecurrenceRuleDayOfPeriod {

        public static implicit operator RecurrenceRuleDayOfYear(string value) {

            RecurrenceRule rule = value;
            return rule as RecurrenceRuleDayOfYear;
        }

        public RecurrenceRuleDayOfYear(int multiplier, TimeInterval frequency, string values) 
            : base(multiplier, frequency, values, false)
        {
        }
    }
}