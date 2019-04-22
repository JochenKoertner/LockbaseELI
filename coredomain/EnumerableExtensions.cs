using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Lockbase.CoreDomain
{
    public static class EnumerableExtensions
    {
        // https://stackoverflow.com/questions/1577822/passing-a-single-item-as-ienumerablet
        public static IEnumerable<T> Yield<T>(this T item) => Enumerable.Repeat(item, 1);
    }
}