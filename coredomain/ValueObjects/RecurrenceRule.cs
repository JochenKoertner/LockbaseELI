using System;
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

            return new RecurrenceRule(multiplier, frequency);
        }

        public RecurrenceRule(int multiplier, TimeInterval frequency) 
        {
            this.Multiplier = multiplier;
            this.Frequency = frequency;
        }

        public int Multiplier { get; private set; }

        public TimeInterval Frequency { get; private set; }

    }
}