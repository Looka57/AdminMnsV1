using System;
using System.IO;
using System.Threading.Tasks;
using AdminMnsV1.Interfaces.IServices;
using AdminMnsV1.Models;
using AdminMnsV1.Models.ViewModels; // Ou l'emplacement correct de CandidatOnboardingViewModel
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting; // Pour IWebHostEnvironment
using Microsoft.AspNetCore.Identity;
using AdminMnsV1.Data.Repositories.Interfaces;
using AdminMnsV1.ViewModels; // Pour UserManager (si IUserRepository n'expose pas UpdateAsync)

namespace AdminMnsV1.Application.Services.Implementation // Assurez-vous du namespace
{
    public class OnboardingService : IOnboardingService
    {
        private readonly IUserRepository _userRepository; // Utilisez votre Repository
        private readonly IWebHostEnvironment _hostEnvironment;
        // Optionnel : Si _userRepository n'expose pas UserManager.UpdateAsync directement, vous pouvez injecter UserManager ici.
        private readonly UserManager<User> _userManager;

        public OnboardingService(IUserRepository userRepository, IWebHostEnvironment hostEnvironment, UserManager<User> userManager)
        {
            _userRepository = userRepository;
            _hostEnvironment = hostEnvironment;
            _userManager = userManager; // Injectez UserManager si nécessaire pour des opérations spécifiques non couvertes par IUserRepository
        }

        public async Task<(bool Succeeded, string ErrorMessage)> ProcessOnboardingAsync(User user, CandidatOnboardingViewModel model, IFormFile? photoFile)
        {
            // La logique de validation métier se trouve ici, pas dans le contrôleur.
            // Si la photo est requise et manquante:
            if (string.IsNullOrEmpty(user.Photo) && photoFile == null)
            {
                return (false, "Une photo de profil est requise pour compléter l'onboarding.");
            }

            // Gérer le téléchargement de la photo SI un nouveau fichier est fourni
            if (photoFile != null && photoFile.Length > 0)
            {
                // Supprimer l'ancienne photo si elle existe et si une nouvelle est téléchargée
                if (!string.IsNullOrEmpty(user.Photo))
                {
                    DeleteOldProfilePhoto(user.Photo);
                }

                // Enregistrer la nouvelle photo
                user.Photo = await SaveProfilePhotoAsync(photoFile);
            }
            // Si photoFile est null et user.Photo n'est pas null, on ne change rien.

            // Mettre à jour les autres propriétés de l'utilisateur
            user.Sexe = model.Sexe;
            user.Nationality = model.Nationality;
            user.Address = model.Address;
            user.City = model.City;
            user.SocialSecurityNumber = model.SocialSecurityNumber;
            user.FranceTravailNumber = model.FranceTravailNumber;
            user.IsOnboardingCompleted = true; // Marque l'onboarding comme complété

            // Enregistrer les changements via le repository (ou UserManager si c'est votreUserRepository)
            // Si votre IUserRepository.UpdateUserAsync appelle _userManager.UpdateAsync, c'est bon.
            bool updateSucceeded = await _userRepository.UpdateUserAsync(user);
            // Ou si UserRepository n'expose pas updateAsync, utilisez directement UserManager:
            // var updateResult = await _userManager.UpdateAsync(user);
            // bool updateSucceeded = updateResult.Succeeded;

            if (updateSucceeded)
            {
                return (true, null); // Succès
            }
            else
            {
                // Si l'utilisateur a été mis à jour via _userManager.UpdateAsync et qu'il y a des erreurs:
                // Vous devrez récupérer les messages d'erreur de _userManager.UpdateAsync si vous l'appelez directement ici.
                // Exemple avec _userManager:
                // string errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
                return (false, "Erreur lors de la mise à jour de votre profil.");
            }
        }

        private async Task<string> SaveProfilePhotoAsync(IFormFile photoFile)
        {
            string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "images", "Profiles"); // Assurez-vous que "Profiles" est correct
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string uniqueFileName = Guid.NewGuid().ToString() + "_" + photoFile.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await photoFile.CopyToAsync(fileStream);
            }

            return uniqueFileName; // Assurez-vous que le chemin est cohérent avec le dossier
        }

        private void DeleteOldProfilePhoto(string oldPhotoPath)
        {
            if (!string.IsNullOrEmpty(oldPhotoPath))
            {
                // oldPhotoPath viendra de currentUser.Photo, qui doit être un chemin relatif (ex: /images/Profiles/...)
                // On retire le premier '/' pour combiner correctement avec WebRootPath
                string oldFilePath = Path.Combine(_hostEnvironment.WebRootPath, oldPhotoPath.TrimStart('/'));
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
            }
        }
    }
}