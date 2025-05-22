// AdminMnsV1.Application.Services/Interfaces/ICandidatureService.cs
using AdminMnsV1.Models.Candidature; // Assure-toi que c'est le bon chemin pour ton modèle Candidature
using AdminMnsV1.Models.ViewModels; // Pour CreateCandidatureViewModel (si dans ViewModels)
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdminMnsV1.Application.Services.Interfaces // <-- TRÈS IMPORTANT : CORRESPONDE AU USING DANS LE CONTRÔLEUR
{
    public interface ICandidatureService
    {
        Task<bool> CreateCandidatureAsync(CreateCandidatureViewModel model);
        Task<IEnumerable<Candidature>> GetAllCandidaturesWithDetailsAsync();
        Task<Candidature?> GetCandidatureByIdWithDetailsAsync(int id);
        Task<int?> GetCandidatureStatusIdByName(string statusName);
        Task<bool> UpdateCandidatureAsync(Candidature candidature); // Si tu as une méthode de mise à jour
        // Ajoute ici toutes les méthodes de logique métier que ton contrôleur ou d'autres services pourraient appeler pour les candidatures.
    }
}