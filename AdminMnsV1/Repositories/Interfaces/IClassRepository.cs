// Repositories/Interfaces/IClassRepository.cs
using AdminMnsV1.Models.Classes;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdminMnsV1.Repositories.Interfaces
{
    public interface IClassRepository
    {
        Task<List<Class>> GetAllClassesAsync();
        Task<Class?> GetClassByIdAsync(int id);
        // Nouvelle méthode pour obtenir les classes avec le nombre d'étudiants
        Task<List<object>> GetClassesWithStudentCountsAsync(); // On utilise object ici pour la flexibilité initiale
    }
}