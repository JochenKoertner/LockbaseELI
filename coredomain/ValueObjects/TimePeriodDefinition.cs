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

            Group startTimeGroup = match.Groups["starttime"];
            Group endTimeGroup = match.Groups["endtime"];
            Group durationGroup = match.Groups["duration"];
            Group recurrenceGroup = match.Groups["recurrence"];

            bool hasStartTime = !String.IsNullOrEmpty(startTimeGroup.Value);
            bool hasEndTime = !String.IsNullOrEmpty(endTimeGroup.Value);
            bool hasDuration = !String.IsNullOrEmpty(durationGroup.Value);
            bool hasRecurrence = !String.IsNullOrEmpty(recurrenceGroup.Value);

            // Wiederholungsregel und Endedatum setzen eine Startzeit und Dauer voraus
            if ((hasRecurrence || hasEndTime) && (!(hasStartTime && hasDuration)))
                throw new ArgumentNullException("starttime & duration", "Reccurency Rule or endtime needs startime and duration");

            var startTime = hasStartTime ? StringToDateTime(startTimeGroup.Value) : default(DateTime?);

            var duration = hasDuration ? Convert.ToInt32(durationGroup.Value) : default(int?);


            var rules = ImmutableArray<RecurrenceRule>.Empty;
            if (hasRecurrence) {
                rules = recurrenceGroup.Value.Split(';').Aggregate(rules, (accu,current) => accu.Add(current));
            }

            var endTime = hasEndTime ? StringToDateTime(endTimeGroup.Value) : default(DateTime?);
            return new TimePeriodDefinition(startTime, duration, endTime, rules);
        }

        public TimePeriodDefinition(DateTime? startTime, int? duration, DateTime? endTime, IImmutableList<RecurrenceRule> recurrenceRules) 
        {
            this.StartTime = startTime;
            this.EndTime = endTime;
            this.Duration = duration;
            this.RecurrenceRules = recurrenceRules; 
        }

        public int? Duration { get; private set; }

        public DateTime? StartTime { get; private set; }
        public IImmutableList<RecurrenceRule> RecurrenceRules { get; private set; }
        public DateTime? EndTime { get; private set; }

        public static DateTime StringToDateTime(string value) {
            return DateTime.ParseExact(value,
                "yyyyMMdd%THHmmss%Z",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None).ToUniversalTime();
        }
    }
}