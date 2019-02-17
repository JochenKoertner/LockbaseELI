using System; 

namespace Lockbase.CoreDomain.Enumerations {

   
    public class IntRange {
        public int Start { get; }
        public int End { get; }

        public IntRange(int start, int end)
        {
            this.Start = start;
            this.End = end;
        }
    }

    public class WeekDaySet {
        
    }

    public sealed class TimeInterval : Enumeration<TimeInterval>
	{
        public string Alias { get; }
		private TimeInterval(int id, string name, string alias)
			: base(id, name)
		{
            this.Alias = alias;
		}

		public bool IsTimesRange => (
			this == Second || this == Minute || this == Hour || 
			this == Month || this == WeekOfYear || this == Year
		);

		public static readonly TimeInterval Second = new TimeInterval(0, "Second", "s");
		public static readonly TimeInterval Minute = new TimeInterval(1, "Minute", "m");
		public static readonly TimeInterval Hour = new TimeInterval(2, "Hour", "h");
		public static readonly TimeInterval DayOfWeek = new TimeInterval(3, "Day of Week", "DW");
		public static readonly TimeInterval DayOfMonth = new TimeInterval(4, "Day of Month", "DM");
		public static readonly TimeInterval DayOfYear = new TimeInterval(5, "Day of Year", "DY");
		public static readonly TimeInterval DayOfWeekPerMonth = new TimeInterval(6, "Day of Week per Month", "DWM");
		public static readonly TimeInterval DayOfWeekPerYear = new TimeInterval(7, "Day of Week per Year", "DWY");
		public static readonly TimeInterval WeekOfYear = new TimeInterval(8, "Week of Year", "WY");
		public static readonly TimeInterval Month = new TimeInterval(9, "Month", "M");
		public static readonly TimeInterval Year = new TimeInterval(10, "Year", "Y");
	}



}
