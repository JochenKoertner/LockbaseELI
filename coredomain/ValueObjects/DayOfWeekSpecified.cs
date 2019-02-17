
using System;

namespace Lockbase.CoreDomain.ValueObjects  {
    
    public class DayOfWeekSpecified {
            
        public DayOfWeek DayOfWeek { get; private set; }
        public int Specifier { get; private set; }

        public DayOfWeekSpecified(DayOfWeek dayOfWeek, int specifier) {
            this.DayOfWeek = dayOfWeek;
            this.Specifier = specifier;
        }
    }
}