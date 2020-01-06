using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions; 

using Lockbase.CoreDomain.Enumerations;

namespace Lockbase.CoreDomain.ValueObjects  {


	// see also  RFC 5545 !! 
	
	[DebuggerDisplay("{DebuggerDisplay,nq}")] // nq means no quote
	public class RecurrenceRule {


		/// implizite Konvertierung vom String
		public static implicit operator RecurrenceRule(string value)
		{
			// https://regex101.com/r/vln7Wv/2/

			const string pattern = @"(?'multiplier'\d*)(?'frequency's|m|h|DWY|DWM|DW|DM|DY|WY|M|Y)(?'values'\(.+\))*";

			RegexOptions options = RegexOptions.IgnoreCase;

			Match match = Regex.Matches(value, pattern, options).Cast<Match>().First();


			Group multiplierGroup = match.Groups["multiplier"];
			Group frequencyGroup = match.Groups["frequency"];
			Group valuesGroup = match.Groups["values"];
			
			int multiplier = String.IsNullOrEmpty(multiplierGroup.Value) ? 1 : Convert.ToInt32(multiplierGroup.Value);
			TimeInterval frequency = TimeInterval.GetAll().SingleOrDefault( ti => ti.Alias == frequencyGroup.Value);

			string values = valuesGroup.Value;

			if (frequency == TimeInterval.DayOfWeek)
				return new RecurrenceRuleDayOfWeek(multiplier, frequency, values);
			else if (frequency == TimeInterval.DayOfWeekPerMonth)
				return new RecurrenceRuleDayOfMonth(multiplier, frequency, values); 
			else if (frequency == TimeInterval.DayOfWeekPerYear)
				return new RecurrenceRuleDayOfYear(multiplier, frequency, values); 
			else if (frequency.IsTimesRange)
				return new RecurrenceRuleTime(multiplier, frequency, values);

			return new RecurrenceRule(multiplier, frequency);
		}

		public RecurrenceRule(int multiplier, TimeInterval frequency) 
		{
			this.Multiplier = multiplier;
			this.Frequency = frequency;
		}


		public int Multiplier { get; private set; }

		public TimeInterval Frequency { get; private set; }

		protected static IEnumerable<T> GetAllEnums<T>()  {
			return Enum.GetValues(typeof(T))
							.Cast<T>();
		}

		protected static DayOfWeek GetDayOfWeekFrom(string shortname) {
			return GetAllEnums<DayOfWeek>()
				.Single( day => shortname.Equals(Enum.GetName(typeof(DayOfWeek), day).Substring(0,shortname.Length),
					StringComparison.OrdinalIgnoreCase));
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		internal string DebuggerDisplay { 
			get {
				var sb = new StringBuilder();
				if (Multiplier != 1)
					sb.Append(Multiplier);
				sb.Append(Frequency.DebuggerDisplay);
				return sb.ToString();
			}
		}

		protected enum Operator {
			Add,
			Range 
		}

		protected readonly struct ReductionState<T> {
			
			public readonly IImmutableSet<T> Reduction;
			public readonly Operator Op;

			public ReductionState(IImmutableSet<T> reduction, Operator op)
			 => (Reduction,Op) = (reduction,op);

			public static ReductionState<T> Default()
				=> new ReductionState<T> (ImmutableHashSet<T>.Empty, Operator.Add);
		}
	}
}