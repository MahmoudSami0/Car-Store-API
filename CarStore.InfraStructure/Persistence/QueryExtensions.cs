using System.Linq.Dynamic.Core;

namespace CarStore.InfraStructure.Persistence
{
    public static class QueryExtensions
    {
        public static IQueryable<T> ApplyFilters<T>(this IQueryable<T> query, string? filter)
        {
            if (string.IsNullOrEmpty(filter)) return query;

            var conditions = filter.Split(";", StringSplitOptions.RemoveEmptyEntries);

            foreach (var condition in conditions)
            {
                if (condition.Contains(":"))
                {
                    var parts = condition.Split(":");
                    if (parts.Length != 2) continue;

                    var prop = parts[0].Trim();
                    var value = parts[1].Trim();

                    query = query.Where($"{prop}.ToString().Contains(@0)", value);
                }
                else if(condition.Contains(">") || condition.Contains("<") || condition.Contains("="))
                {
                    query = query.Where(condition);
                }
            }

            return query;
        }

        public static IQueryable<T> ApplySort<T>(this IQueryable<T> query, string? sort)
        {
            if(string.IsNullOrEmpty(sort)) return query;

            var parts = sort.Split(":");
            var prop = parts[0].Trim();
            var direction = parts.Length == 2 ? parts[1].Trim() : "asc";

            return query.OrderBy($"{prop} {direction}");
        }

        public static IQueryable<T> ApplyPagination<T>(this IQueryable<T> query, int page, int pageSize)
        {
            page = page < 1 ? 1 : page;
            if (pageSize < 1) pageSize = 10;
            else if (pageSize > 40) pageSize = 40;
            
            return query.Skip((page - 1) * pageSize).Take(pageSize);
        }
    }
}