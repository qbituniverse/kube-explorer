using System.Collections.Generic;
using System.Linq;

namespace qu.nuget.infrastructure.Extensions
{
    public static class GlobalExtensions
    {
        public static bool Exists<T>(this T obj) => obj != null;

        public static bool HasValues<T>(this List<T> list) => list?.Count > 0;

        public static List<T> Exclude<T>(this List<T> list, List<T> toExclude) => list.Except(toExclude).ToList();
    }
}