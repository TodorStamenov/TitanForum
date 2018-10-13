namespace TitaniumForum.Data.Infrastructure.Extensions
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    public static class QueryExtensions
    {
        public static IQueryable<T> FilterElements<T>(this IQueryable<T> query, Expression<Func<T, bool>> expression)
        {
            return expression != null
                ? query.Where(expression)
                : query;
        }

        public static IQueryable<T> OrderElements<T>(this IQueryable<T> query, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy)
        {
            return orderBy != null
                ? orderBy(query)
                : query;
        }

        public static IQueryable<T> SkipElements<T>(this IQueryable<T> query, int? count)
        {
            return count.HasValue
                ? query.Skip(count.Value)
                : query;
        }

        public static IQueryable<T> TakeElements<T>(this IQueryable<T> query, int? count)
        {
            return count.HasValue
                ? query.Take(count.Value)
                : query;
        }

        public static int CountElements<T>(this IQueryable<T> query, Expression<Func<T, bool>> expression)
        {
            return expression != null
                ? query.Count(expression)
                : query.Count();
        }
    }
}