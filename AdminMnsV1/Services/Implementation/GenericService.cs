// Services/GenericService.cs
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Collections.Generic;
using AdminMnsV1.Repositories.Interfaces; // Pour IGenericRepository
using AdminMnsV1.Services.Interfaces; // Pour IGenericService

namespace AdminMnsV1.Services
{
    public class GenericService<T> : IGenericService<T> where T : class
    {
        protected readonly IGenericRepository<T> _repository;

        public GenericService(IGenericRepository<T> repository)
        {
            _repository = repository;
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }


        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _repository.FindAsync(predicate);
        }

        public async Task<T> AddAsync(T entity)
        {
            _repository.Add(entity);
            await _repository.SaveChangesAsync();
            return entity;
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            _repository.AddRange(entities);
            await _repository.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            _repository.Update(entity);
            await _repository.SaveChangesAsync();
        }

        public async Task DeleteAsync(T entity)
        {
            _repository.Delete(entity);
            await _repository.SaveChangesAsync();
        }

        public async Task RemoveRangeAsync(IEnumerable<T> entities)
        {
            _repository.RemoveRange(entities);
            await _repository.SaveChangesAsync();
        }

        public Task<T?> GetByIdAsync(string id)
        {
            throw new NotImplementedException();
        }
    }
}