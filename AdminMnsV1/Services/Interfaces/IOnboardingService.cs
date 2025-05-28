using System.Threading.Tasks;
using AdminMnsV1.Models;
using AdminMnsV1.Models.ViewModels; // Ou l'emplacement correct de CandidatOnboardingViewModel
using AdminMnsV1.ViewModels;
using Microsoft.AspNetCore.Http; // Pour IFormFile

namespace AdminMnsV1.Interfaces.IServices
{
    public interface IOnboardingService
    {
        // Cette méthode prend l'utilisateur actuel, le modèle du formulaire, et le fichier photo.
        Task<(bool Succeeded, string ErrorMessage)> ProcessOnboardingAsync(User user, CandidatOnboardingViewModel model, IFormFile? photoFile);

        // Les méthodes de gestion de fichiers peuvent être privées dans le service ou dans un service de fichiers séparé si réutilisable
        // Task<string> SaveProfilePhotoAsync(IFormFile photoFile, string existingPhotoPath);
        // void DeleteOldProfilePhoto(string oldPhotoPath);
    }
}