using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions; 

using Lockbase.CoreDomain.Enumerations;

namespace Lockbase.CoreDomain.ValueObjects  {


    // see also  RFC 5545 !! 
    
    public class TimePeriodDefinition {
        /// implizite Konvertierung vom String
        public static implicit operator TimePeriodDefinition(string value)
        {
            // https://regex101.com/r/UJhaaC/2

            const string pattern = @"((?'starttime'\d{8}T\d{6}Z)?(\/(?'duration'\d*)?(\/(?'recurrence'(\d*)(s|m|h|DWY|DWM|DW|DM|WY|M|Y)(\(.+\))*)?(\/(?'endtime'\d{8}T\d{6}Z))?)?)?)?";

            RegexOptions options = RegexOptions.IgnoreCase;

            Match match = Regex.Matches(value, pattern, options).First();


            Group starttimeGroup = match.Groups["starttime"];
            Group endtimeGroup = match.Groups["endtime"];
            Group durationGroup = match.Groups["duration"];
            Group recurenceGroup = match.Groups["recurrence"];


            var startTime = String.IsNullOrEmpty(starttimeGroup.Value) ?
                default(DateTime?) : StringToDateTime(starttimeGroup.Value);
            
            var rule = String.IsNullOrEmpty(recurenceGroup.Value) ?
                default(RecurrenceRule) : (RecurrenceRule)recurenceGroup.Value;

            var endTime = String.IsNullOrEmpty(endtimeGroup.Value) ?
                default(DateTime?) : StringToDateTime(endtimeGroup.Value);


            return new TimePeriodDefinition(startTime, string.IsNullOrEmpty(durationGroup.Value) ?
                default(int?) : Convert.ToInt32(durationGroup.Value), rule, endTime);
        }

        public TimePeriodDefinition(DateTime? startTime, int? duration, RecurrenceRule recurrenceRule, DateTime? endTime) 
        {
            this.StartTime = startTime;
            this.EndTime = endTime;
            this.Duration = duration;
            this.RecurrenceRule = recurrenceRule; 
        }


        public int? Duration { get; private set; }

        public DateTime? StartTime { get; private set; }
        public RecurrenceRule RecurrenceRule { get; private set; }
        public DateTime? EndTime { get; private set; }

        public static DateTime StringToDateTime(string value) {
            return DateTime.ParseExact(value,
                "yyyyMMdd%THHmmss%Z",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None).ToUniversalTime();
        }
    }

}