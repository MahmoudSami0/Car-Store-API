using System.Linq.Expressions;

namespace CarStore.Application.Interfaces;

public interface IBaseRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(Guid id);
    Task<T?> FindAsync(Expression<Func<T,bool>> criteria, string[]? includes = null);
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(Guid id);
    Task<List<TResult>> CustomFindAsync<TEntity, TResult>(
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IQueryable<TEntity>> includes = null,
        CancellationToken cancellationToken = default)
        where TEntity : class;
}