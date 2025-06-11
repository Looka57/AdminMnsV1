using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using AdminMnsV1.Data; // Assurez-vous que ce chemin est correct pour votre DbContext
using AdminMnsV1.Repositories.Interfaces; // Assurez-vous que ce chemin est correct pour IGenericRepository
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdminMnsV1.Repositories.Implementation
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class

    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        // --- Méthodes asynchrones ---
        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetAllAbsenceAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        // --- Méthodes synchrones ---
        public void Add(T entity)
        {
            _dbSet.Add(entity);
        }

        public void AddRange(IEnumerable<T> entities)
        {
            _dbSet.AddRange(entities);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }

        // --- Méthode de sauvegarde ---
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}