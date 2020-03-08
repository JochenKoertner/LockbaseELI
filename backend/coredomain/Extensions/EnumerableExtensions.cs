using System.Collections.Generic;
using System.Linq;

namespace Lockbase.CoreDomain.Extensions
{
    public static class EnumerableExtensions
	{
		// https://stackoverflow.com/questions/1577822/passing-a-single-item-as-ienumerablet
		public static IEnumerable<T> Yield<T>(this T item) => Enumerable.Repeat(item, 1);

		// https://stackoverflow.com/questions/2094729/recommended-way-to-check-if-a-sequence-is-empty
		public static bool IsEmpty<T>(this IEnumerable<T> source) => !source.Any();
	}
}