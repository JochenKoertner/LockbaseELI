using System;

namespace Lockbase.CoreDomain.ValueObjects  {
	
	public readonly struct DayOfWeekSpecified : IEquatable<DayOfWeekSpecified> {
			
		public readonly DayOfWeek DayOfWeek;
		public readonly int Specifier;

		public DayOfWeekSpecified(DayOfWeek dayOfWeek, int specifier) 
			=> (DayOfWeek, Specifier) = (dayOfWeek, specifier);

		public bool Equals(DayOfWeekSpecified other)
		{
			if (this.DayOfWeek == other.DayOfWeek && this.Specifier == other.Specifier)
				return true;
			 else
				 return false;
		}
	}
}