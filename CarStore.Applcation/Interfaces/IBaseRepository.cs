using CarStore.Applcation.DTOs.Pagination;
using System.Linq.Expressions;

namespace CarStore.Application.Interfaces;

public interface IBaseRepository<T> where T : class
{
    Task<IQueryable<T?>> GetAllAsync();
    Task<T?> GetByIdAsync(Guid id);
    Task<T?> FindAsync(Expression<Func<T,bool>> criteria, string[]? includes = null);
    Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> criteria);
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
    Task DeleteRangeAsync(ICollection<T> entities);
    Task DeleteWhereAsync(Expression<Func<T, bool>> criteria);
    Task<List<TResult>> CustomFindAsync<TEntity, TResult>(
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IQueryable<TEntity>> includes = null,
        CancellationToken cancellationToken = default)
        where TEntity : class;
}