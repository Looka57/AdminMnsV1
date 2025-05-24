using AdminMnsV1.Models.Classes;
using System.Collections.Generic; // IMPORTANT : utiliser System.Collections.Generic pour IEnumerable<T>
using System.Threading.Tasks;

namespace AdminMnsV1.Repositories.Interfaces
{
    // IClassRepository hérite de notre IGenericRepository pour toutes les opérations de base
    public interface IClassRepository : IGenericRepository<SchoolClass>
    {
        // Méthode spécifique aux classes, avec le bon type de retour générique
        Task<IEnumerable<SchoolClass>> GetAllClassesAsync();

        // Si cette méthode est spécifique à IClassRepository, elle reste ici
        Task<IEnumerable<object>> GetClassesWithStudentCountsAsync();
    }
}