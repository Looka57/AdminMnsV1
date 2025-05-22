using AdminMnsV1.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using AdminMnsV1.Repositories.Interfaces; // Assure-toi que ce using est correct selon ta structure

namespace AdminMnsV1.Repositories.Implementation
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        // C'est le constructeur correct et attendu.
        // Il ne prend qu'un seul paramètre: ApplicationDbContext context
        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>(); // L'initialisation de _dbSet se fait ici
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public void Add(T entity)
        {
            _dbSet.Add(entity);
        }

        // Attention, tu avais AddRang au lieu de AddRange
        public void AddRange(IEnumerable<T> entities) // <-- Correction ici
        {
            _dbSet.AddRange(entities);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        // Cette méthode GetAllAsync(predicate) est redondante avec FindAsync.
        // Je te recommande de la supprimer ou d'utiliser FindAsync à la place.
        // Si tu la gardes, tu dois la définir dans l'interface IGenericRepository.
        /*
        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }
        */
    }
}