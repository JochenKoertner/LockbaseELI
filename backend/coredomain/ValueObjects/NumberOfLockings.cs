
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Lockbase.CoreDomain.ValueObjects {

	public class NumberOfLockings {
		/// implizite Konvertierung vom String
		public static implicit operator NumberOfLockings(string value)
		{
			// https://regex101.com/r/nrxVfq/2
			string pattern = @"((?'MinNmbOfLockings'\d+)?(-(?'MaxNmbOfLockings'\d+)?)?)?";

			RegexOptions options = RegexOptions.IgnoreCase;

			Match match = Regex.Matches(value, pattern, options).Cast<Match>().First();

			Group minNumberGroup = match.Groups["MinNmbOfLockings"];
			Group maxNumberGroup = match.Groups["MaxNmbOfLockings"];

			bool hasMin = !String.IsNullOrEmpty(minNumberGroup.Value);
			bool hasMax = !String.IsNullOrEmpty(maxNumberGroup.Value);

			if (hasMin || hasMax) {
				var minNumber = hasMin ? Convert.ToInt32(minNumberGroup.Value) : 0;
				var maxNumber = hasMax ? Convert.ToInt32(maxNumberGroup.Value) : minNumber;

				return new NumberOfLockings(minNumber, maxNumber); 
			}

			return null;
		}

		 public int Minimum { get; private set; }
		 public int Maximum { get; private set; }

		public NumberOfLockings(int minimum, int maximum) 
		{
			if (maximum < minimum) 
				throw new ArgumentOutOfRangeException(nameof(maximum), maximum, 
					$"'{nameof(maximum)}' must not be less '{nameof(minimum)}'");
			this.Minimum = minimum;
			this.Maximum = maximum;
		}
	}
}