﻿namespace Lucassen.Extensions
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using Helpers;

    public static class IQueryableExtensions
    {
        public static IQueryable<T> OrderByProperty<T>(this IQueryable<T> source, string propertyName, bool isDescending) where T : class
        {
            if (source == null) return source;
            if (propertyName == null) return source;

            var methodName = isDescending ? "OrderByDescending" : "OrderBy";
            Type type;
            var lambda = LambdaHelper.GenerateSelector<T>(propertyName, out type);

            var result = typeof(Queryable).GetMethods().Single(
                    method => method.Name == methodName
                              && method.IsGenericMethodDefinition
                              && method.GetGenericArguments().Length == 2
                              && method.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(T), type)
                .Invoke(null, new object[] {source, lambda});
            return (IQueryable<T>) result;
        }

        public static IQueryable<T> WherePropertiesContain<T>(this IQueryable<T> source, Expression<Func<T, object>>[] propertyNames, string query) where T : class
        {
            if (source == null) return source;
            if (propertyNames.Length == 0) return source;

            var lambda = LambdaHelper.GenerateWhere(propertyNames, query);
            return source.Where(lambda);
        }
    }
}