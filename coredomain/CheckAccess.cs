using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Lockbase.CoreDomain.ValueObjects;

// https://mikhail.io/2016/01/validation-with-either-data-type-in-csharp/

// https://github.com/mikhailshilkov/mikhailio-samples/blob/master/Either%7BTL%2CTR%7D.cs


namespace Lockbase.CoreDomain {

    public static class CheckAccess {
        
        public static bool Check(IEnumerable<TimePeriodDefinition> timeDefinitions, DateTime time) {
            var definition = timeDefinitions.SingleOrDefault(
                td => td.StartTime <= time && time <= td.EndTime);  

            if (definition == null)
                return false;
            return Check(definition, time);
        }

        public static bool Check(TimePeriodDefinition timePeriodDefinition, DateTime time) {

            if (timePeriodDefinition.Duration.HasValue && timePeriodDefinition.StartTime.HasValue)
            {
                TimeSpan start = timePeriodDefinition.StartTime.Value.TimeOfDay;
                var allowed = timePeriodDefinition.Duration.Value;
                var current = time.TimeOfDay.Subtract(start).TotalSeconds;
                if (current > allowed || current < 0)
                    return false;
            }
            return true;
        } 
    }
}