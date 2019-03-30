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
            return true;
        }
    }
}