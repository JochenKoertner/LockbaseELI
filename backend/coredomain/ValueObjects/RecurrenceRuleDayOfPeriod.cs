using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions; 

using Lockbase.CoreDomain.Enumerations;

namespace Lockbase.CoreDomain.ValueObjects  {
	
	public abstract class RecurrenceRuleDayOfPeriod : RecurrenceRule {

		public RecurrenceRuleDayOfPeriod(int multiplier, TimeInterval frequency, string values, bool isMonth) : base(multiplier, frequency)
		{
			this.WeekDays = GetWeekOfDaySet(values, isMonth);
		}

		public IImmutableSet<DayOfWeekSpecified> WeekDays { get; private set; }


		private static IImmutableSet<DayOfWeekSpecified> GetWeekOfDaySet(string values, bool isMonth) {
			
			if (string.IsNullOrEmpty(values))
				return ImmutableHashSet<DayOfWeekSpecified>.Empty.AddRange(GetAllEnums<DayOfWeek>().Select( day => new DayOfWeekSpecified(day, 0)));
	
			
			var seed = ReductionState<DayOfWeekSpecified>.Default();

			// https://regex101.com/r/lbGStS/2
			RegexOptions options = RegexOptions.IgnoreCase;
			string input = values.Substring(1, values.Length - 2);
			string pattern = $"([+])|([Mo|Tu|We|Th|Fr|Sa|Su]+{GetSpecifierRegexPattern(isMonth)})";
			
			return Regex.Matches(input, pattern, options)
				.Cast<Match>()
				.Select(  match => match.Value )
				.Aggregate(seed, (accu,current) => 
					{
						if (current == "+")
						{
							return new ReductionState<DayOfWeekSpecified>(accu.Reduction, Operator.Add);
						}
						var operand = DayOfWeekSpecifiedFromValue(current, isMonth);

						return new ReductionState<DayOfWeekSpecified>( accu.Reduction.Add(operand), accu.Op);
					})
				.Reduction;  
		}

		private static DayOfWeekSpecified DayOfWeekSpecifiedFromValue(string value, bool isMonth) {
			string pattern = $"(Mo|Tu|We|Th|Fr|Sa|Su)({GetSpecifierRegexPattern(isMonth)})";
			RegexOptions options = RegexOptions.IgnoreCase;
			var parts = Regex
				.Split(value, pattern, options)
				.Where( s => !string.IsNullOrEmpty(s))
				.ToArray();
			
			var alias = parts.First();
			var specifier = Convert.ToInt32(parts.Last());

			var dayOfWeek = GetDayOfWeekFrom(alias);
			
			return new DayOfWeekSpecified(dayOfWeek, specifier);
		}

		private static string GetSpecifierRegexPattern(bool isMonth) {
			return isMonth ? @"-?\d{1}" : @"-?\d{1,2}";
		}
	}
}