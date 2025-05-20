// Services/Interfaces/IClassService.cs
using AdminMnsV1.Models; // Pour CardModel
using AdminMnsV1.Models.ViewModels; // IMPORTANT : Pour ClassListViewModel
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdminMnsV1.Services.Interfaces
{
    public interface IClassService
    {
        Task<List<CardModel>> GetClassCardModelsAsync(); // Gardez si elle est toujours utilisée
        Task<ClassListViewModel> GetClassListPageViewModelAsync(); // Nouvelle méthode
    }
}