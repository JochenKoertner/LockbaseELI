
using System;

namespace Lockbase.CoreDomain.ValueObjects  {
    
    public class DayOfWeekSpecified : IEquatable<DayOfWeekSpecified> {
            
        public DayOfWeek DayOfWeek { get; private set; }
        public int Specifier { get; private set; }

        public DayOfWeekSpecified(DayOfWeek dayOfWeek, int specifier) {
            this.DayOfWeek = dayOfWeek;
            this.Specifier = specifier;
        }

        public bool Equals(DayOfWeekSpecified other)
        {
            if (other == null)
                return false;

            if (this.DayOfWeek == other.DayOfWeek && this.Specifier == other.Specifier)
                return true;
             else
                 return false;
        }
    }
}