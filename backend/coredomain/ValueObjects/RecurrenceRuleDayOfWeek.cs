using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions; 

using Lockbase.CoreDomain.Enumerations;
using Lockbase.CoreDomain.Extensions;

namespace Lockbase.CoreDomain.ValueObjects  {
	
	[DebuggerDisplay("{DebuggerDisplay,nq}")] // nq means no quote

	public class RecurrenceRuleDayOfWeek : RecurrenceRule {

		public static implicit operator RecurrenceRuleDayOfWeek(string value) {

			RecurrenceRule rule = value;
			return rule as RecurrenceRuleDayOfWeek;
		}

		public RecurrenceRuleDayOfWeek(int multiplier, TimeInterval frequency, string values) : base(multiplier, frequency)
		{
			this.WeekDays = GetWeekOfDaySet(values);
		}

		public IImmutableSet<DayOfWeek> WeekDays { get; private set; }


		private static IImmutableSet<DayOfWeek> GetWeekOfDaySet(string values) {
			
			if (string.IsNullOrEmpty(values))
				return ImmutableHashSet<DayOfWeek>.Empty.AddRange(GetAllEnums<DayOfWeek>());
			
			var seed = ReductionState<DayOfWeek>.Default();

			// https://regex101.com/r/4NRHND/2
			RegexOptions options = RegexOptions.IgnoreCase;
			string input = values.Substring(1, values.Length - 2);
			string pattern = @"([+\-])|(Mo|Tu|We|Th|Fr|Sa|Su)";
			
			return Regex.Matches(input, pattern, options)
				.Cast<Match>()
				.Select(  match => match.Value )
				.Aggregate(seed, (accu,current) => 
					{
						if (current == "+" || current == "-")
						{
							return new ReductionState<DayOfWeek>( accu.Reduction, (current == "+") ? Operator.Add : Operator.Range );
						}
						var operand = GetDayOfWeekFrom(current);

						if (accu.Op == Operator.Add)
							return new ReductionState<DayOfWeek>( accu.Reduction.Add(operand), accu.Op);

						var lastOperand = accu.Reduction.Last();

						var range = GetRange(lastOperand, operand);
							
						return new ReductionState<DayOfWeek>( accu.Reduction.AddRange(range), Operator.Add );  
					})
				.Reduction;
		}

		private static IEnumerable<DayOfWeek> GetRange(DayOfWeek start, DayOfWeek end) {
			// In Germany the week start on Monday so we must handle a Range Friday..Sunday correctly
			var totalWeek = GetAllEnums<DayOfWeek>();
			var range = totalWeek
							.Concat(totalWeek)
							.SkipWhile( day => day != start)
							.TakeWhile( day => day != end)
							.Concat(new []{end});
			return range;
		}   

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		internal new string DebuggerDisplay { 
			get {
				var weekdays = String.Join("+",
					WeekDays.Select( wd => wd.ToString().Substring(0,2)));
				return $"{base.DebuggerDisplay}({weekdays})";
			}
		}     
	}
}