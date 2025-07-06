using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace AdminMnsV1.Repositories.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAbsenceAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        


        void Add(T entity);
        void AddRange(IEnumerable<T> entities);
        void Update(T entity);
        void Delete(T entity);
        void RemoveRange(IEnumerable<T> entities);

        Task<int> SaveChangesAsync();
        Task<IEnumerable<T>> GetAllAsync();
    }
}