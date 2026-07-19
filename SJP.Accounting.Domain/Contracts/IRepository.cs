using System.Linq.Expressions;

namespace SJP.Accounting.Domain.Contracts;

public interface IRepository<T> where T : class
{
    Task<T?> GetBySysIdAsync(string id);

    Task<T?> FirstOrDefaultAsync(
        Expression<Func<T, bool>>? predicate = null,
        params Expression<Func<T, object>>[] includes);

    Task<IEnumerable<T>> ListAsync(
        Expression<Func<T, bool>>? predicate = null,
        params Expression<Func<T, object>>[] includes);

    Task<int> CountAsync();

    Task<int> CountAsync(
        Expression<Func<T, bool>> predicate);

    Task<bool> AnyAsync(
        Expression<Func<T, bool>> predicate);

    Task AddAsync(T entity);

    Task AddRangeAsync(IEnumerable<T> entities);

    Task UpdateAsync(T entity);

    Task UpdateRangeAsync(IEnumerable<T> entities);

    Task RemoveAsync(T entity);

    Task RemoveRangeAsync(IEnumerable<T> entities);

    Task SaveChangesAsync();
}