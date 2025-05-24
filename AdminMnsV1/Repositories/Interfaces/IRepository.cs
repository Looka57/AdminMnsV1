// Repositories/Interfaces/IRepository.cs
using System.Collections.Generic; // Ajoutez cette directive using si elle manque
using System.Threading.Tasks;    // Ajoutez cette directive using si elle manque

namespace AdminMnsV1.Repositories.Interfaces
{
    public interface IRepository<TEntity> where TEntity : class
    {
        // DÉCLAREZ LA MÉTHODE GetAllAsync ICI
        Task<IEnumerable<TEntity>> GetAllAsync();

        // Vous devriez probablement aussi ajouter d'autres méthodes génériques ici
        // si votre application les utilise :
        Task<TEntity?> GetByIdAsync(int id);
        Task AddAsync(TEntity entity);
        Task UpdateAsync(TEntity entity); // Ou Task UpdateAsync(TEntity entity); selon votre implémentation
      
        Task DeleteAsync(int id);
        Task<int> SaveChangesAsync();
    }
}