namespace Lucassen.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Helpers;

    public static class ListHelper
    {
        public static IEnumerable<T> OrderByProperty<T>(this IEnumerable<T> source, string propertyName, bool isDescending)
            where T : class
        {
            var q = source.AsQueryable();
            Type type;
            var exp = LambdaHelper.GenerateSelector<T>(propertyName, out type);
            var method = !isDescending ? "OrderBy" : "OrderByDescending";
            var types = new[] {q.ElementType, exp.Body.Type};
            var mce = Expression.Call(typeof(Queryable), method, types, q.Expression, exp);
            return q.Provider.CreateQuery<T>(mce).ToList();
        }

        public static bool IsSame<T>(this IEnumerable<T> set1, IEnumerable<T> set2) where T : IComparable
        {
            if (set1 == null && set2 == null)
                return true;
            if (set1 == null || set2 == null)
                return false;

            var list1 = set1.ToList();
            var list2 = set2.ToList();

            if (list1.Count != list2.Count)
                return false;

            list1.Sort();
            list2.Sort();

            return list1.SequenceEqual(list2);
        }

        public static string ToCsv<T>(this IEnumerable<T> list)
        {
            var properties = typeof(T).GetProperties();
            return $"{string.Join(",", properties.Select(p => p.Name))}{Environment.NewLine}{string.Join(Environment.NewLine, list.Select(i => string.Join(",", properties.Select(p => p.GetValue(i)))))}";
        }
    }
}