using System;
using System.Collections.Generic;
using System.Linq;

namespace IEvangelist.GitHub.Services.Extensions
{
    static class EnumerableExtensions
    {
        public static T GetRandomElement<T>(this IEnumerable<T> source) =>
            source.GetRandomElements(1).Single();

        public static IEnumerable<T> GetRandomElements<T>(this IEnumerable<T> source, int count) =>
            source.Shuffle().Take(count);

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source) => 
            source.OrderBy(x => Guid.NewGuid());
    }
}