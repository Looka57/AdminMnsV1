// Services/Interfaces/IClassService.cs
using AdminMnsV1.Models; // Pour CardModel
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdminMnsV1.Services.Interfaces
{
    public interface IClassService
    {
        // Cette méthode va récupérer les données et les transformer en CardModel
        Task<List<CardModel>> GetClassCardModelsAsync();
        // Tu pourrais ajouter d'autres méthodes ici si tu as des opérations CRUD pour les classes (créer, modifier, supprimer)
    }
}