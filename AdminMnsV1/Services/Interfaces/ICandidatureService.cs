// AdminMnsV1.Application.Services/Interfaces/ICandidatureService.cs
using AdminMnsV1.Models;
using AdminMnsV1.Models.Documents;

using AdminMnsV1.Models.CandidaturesModels;
using AdminMnsV1.Models.ViewModels; // Pour CreateCandidatureViewModel (si dans ViewModels)
using System.Collections.Generic;
using System.Threading.Tasks;


namespace AdminMnsV1.Application.Services.Interfaces 
{
    public interface ICandidatureService
    {
        Task<bool> CreateCandidatureAsync(CreateCandidatureViewModel model);
        Task<IEnumerable<Candidature>> GetAllCandidaturesWithDetailsAsync();
        Task<IEnumerable<CandidatureStudentViewModel>> GetAllCandidaturesForOverviewAsync();

        Task<Candidature?> GetCandidatureByIdWithDetailsAsync(int id);
        Task<int?> GetCandidatureStatusIdByName(string statusName);

        Task<Candidature> GetCandidatureByUserIdAsync(string userId);

        Task<CandidatureStatus> GetCandidatureStatusByIdAsync(int statusId); 
        Task<bool> UpdateCandidatureAsync(Candidature candidature); // Si tu as une méthode de mise à jour
        Task<CandidatureStudentViewModel> GetCandidatureDetailsAsync(int candidatureId);

        Task<CandidatureStudentViewModel> GetCandidatureDetailsByUserIdAsync(string userId);


        Task<bool> UpdateCandidatureStatusAsync(int candidatureId, string newStatus);
        Task<bool> UploadDocumentAsync(int candidatureId, IFormFile document, string documentTypeName);
        Task<int> ValidateDocumentAsync(int documentId, string adminUserId);
        Task<int> RejectDocumentAsync(int documentId, string adminUserId);
        Task<bool> DeleteCandidatureAsync(int id);
        Task<bool> UploadDocumentAsync(int candidatureId, IFormFile document, object documentTypeName);
       
    }
}