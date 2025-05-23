// AdminMnsV1.Application.Services/Interfaces/IClassService.cs
using AdminMnsV1.Models;
using AdminMnsV1.Models.Classes; // Assure-toi que c'est le bon chemin pour ton modèle SchoolClass
using AdminMnsV1.Models.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdminMnsV1.Application.Services.Interfaces // Ou AdminMnsV1.Services.Interfaces
{
    public interface IClassService
    {
        Task<IEnumerable<SchoolClass>> GetAllClassesAsync();
        Task<SchoolClass?> GetClassByIdAsync(int id);
        Task<bool> AddClassAsync(SchoolClass schoolClass);
        Task<bool> UpdateClassAsync(SchoolClass schoolClass);
        Task<bool> DeleteClassAsync(int id);

        Task<ClassListViewModel> GetClassListPageViewModelAsync();

        // Si tu as cette méthode et que tu l'utilises, garde-la aussi dans l'interface
        //Task<List<CardModel>> GetClassCardModelsAsync();
    }
}