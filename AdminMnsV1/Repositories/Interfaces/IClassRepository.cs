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
        // Ajoute d'autres méthodes CRUD si nécessaire pour Class
    }
}