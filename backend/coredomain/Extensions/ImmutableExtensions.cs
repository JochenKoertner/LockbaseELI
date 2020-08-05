using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Lockbase.CoreDomain.Extensions
{

    public static class ImmutableExtensions {

        public static IImmutableSet<T> AddRange<T>(this IImmutableSet<T> immutableSet, IEnumerable<T> range) 
            => range.Aggregate(immutableSet, (accu, current) => accu.Add(current));

    }
}