namespace Meyer.Socrates.Helper
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    internal static class LinqHelper
    {
        // TODO: Add predicate overloads to First(), FirstOrDefault().. etc.

        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> source, T prepended)
        {
            yield return prepended;
            foreach (var item in source)
                yield return item;
        }

        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> source, params T[] prepended)
        {
            foreach (var item in prepended)
                yield return item;
            foreach (var item in source)
                yield return item;
        }

        public static IEnumerable<T> Append<T>(this IEnumerable<T> source, T appended)
        {
            foreach (var item in source)
                yield return item;
            yield return appended;
        }

        public static IEnumerable<T> Append<T>(this IEnumerable<T> source, params T[] appended)
        {
            foreach (var item in source)
                yield return item;
            foreach (var item in appended)
                yield return item;
        }

        public static T First<T>(this IEnumerable source)
        {
            return source.OfType<T>().First();
        }

        public static T FirstOrDefault<T>(this IEnumerable source)
        {
            return source.OfType<T>().FirstOrDefault();
        }

        public static T Single<T>(this IEnumerable source)
        {
            return source.OfType<T>().Single();
        }

        public static T SingleOrDefault<T>(this IEnumerable source)
        {
            return source.OfType<T>().SingleOrDefault();
        }

        public static T Last<T>(this IEnumerable source)
        {
            return source.OfType<T>().Last();
        }

        public static T Last<T>(this IEnumerable source, Func<T, bool> predicate)
        {
            return source.OfType<T>().Last(predicate);
        }

        public static T LastOrDefault<T>(this IEnumerable source)
        {
            return source.OfType<T>().LastOrDefault();
        }

        public static T LastOrDefault<T>(this IEnumerable source, Func<T, bool> predicate)
        {
            return source.OfType<T>().LastOrDefault(predicate);
        }
    }
}
