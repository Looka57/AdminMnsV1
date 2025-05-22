using System.Linq.Expressions;
using Microsoft.CodeAnalysis.Completion;

namespace AdminMnsV1.Repositories.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id); // Récupère un élément par son ID (T peut être Candidature, Document, etc.)
        Task<IEnumerable<T>> GetAllAsync(); // Récupère TOUS les éléments de ce type
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate); // Trouve des éléments selon un critère (ex: c => c.Status == "En cours")

        void Add(T entity); // Ajoute un nouvel élément
        void AddRange(IEnumerable<T> entities); // Ajoute plusieurs éléments à la fois
        void Update(T entity); // Met à jour un élément existant
        void Remove(T entity); // Supprime un élément
        void RemoveRange(IEnumerable<T> entities); // Supprime plusieurs éléments
        Task<int> SaveChangesAsync(); // Enregistre les modifications dans la base de données

    }
}
