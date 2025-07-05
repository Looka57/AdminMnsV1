using System.Linq.Expressions;

namespace AdminMnsV1.Services.Interfaces
{
    public interface IGenericService<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);
        Task<T?> GetByIdAsync(string id); // Pour les entités avec IDs string

        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

        Task<T> AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task RemoveRangeAsync(IEnumerable<T> entities);
    }
   
}
