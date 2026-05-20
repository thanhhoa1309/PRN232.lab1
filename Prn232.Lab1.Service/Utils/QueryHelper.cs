using System.Linq.Expressions;

namespace Prn232.Lab1.Service.Utils
{
    public static class QueryHelper
    {
        public static IQueryable<T> ApplySorting<T>(IQueryable<T> query, string? sortBy, bool isDescending, IDictionary<string, Expression<Func<T, object>>> map)
        {
            if (string.IsNullOrWhiteSpace(sortBy))
            {
                return query;
            }

            if (!map.TryGetValue(sortBy, out var selector))
            {
                return query;
            }

            return isDescending ? query.OrderByDescending(selector) : query.OrderBy(selector);
        }

        public static IQueryable<T> ApplySorting<T>(IQueryable<T> query, string? sort, IDictionary<string, Expression<Func<T, object>>> map)
        {
            if (string.IsNullOrWhiteSpace(sort))
            {
                return query;
            }

            IOrderedQueryable<T>? ordered = null;
            var tokens = sort.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            foreach (var token in tokens)
            {
                var descending = token.StartsWith("-", StringComparison.Ordinal);
                var key = descending ? token[1..] : token;

                if (!map.TryGetValue(key, out var selector))
                {
                    continue;
                }

                if (ordered == null)
                {
                    ordered = descending ? query.OrderByDescending(selector) : query.OrderBy(selector);
                }
                else
                {
                    ordered = descending ? ordered.ThenByDescending(selector) : ordered.ThenBy(selector);
                }
            }

            return ordered ?? query;
        }
    }
}
