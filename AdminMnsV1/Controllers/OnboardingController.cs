using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity; // Pour UserManager et SignInManager
using AdminMnsV1.Models;
using AdminMnsV1.ViewModels;
using System.Threading.Tasks;
using System.IO; // Pour Path, Directory, FileStream
using Microsoft.AspNetCore.Hosting; // Pour IWebHostEnvironment

namespace AdminMnsV1.Controllers
{
    [Authorize(Roles = "Student")] // Seuls les utilisateurs ayant le rôle "Student" peuvent accéder à ce contrôleur
    public class OnboardingController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager; // Ajoutez la déclaration pour SignInManager
        private readonly IWebHostEnvironment _hostEnvironment;

        // Modifiez le constructeur pour injecter SignInManager et IWebHostEnvironment
        public OnboardingController(UserManager<User> userManager, SignInManager<User> signInManager, IWebHostEnvironment hostEnvironment)
        {
            _userManager = userManager;
            _signInManager = signInManager; // Initialisez SignInManager
            _hostEnvironment = hostEnvironment; // Initialisez IWebHostEnvironment
        }

        [HttpGet]
        public async Task<IActionResult> Onboarding()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            // Vérifie si l'utilisateur est bien un candidat et si l'onboarding est déjà complété
            if (currentUser == null || currentUser.Status != "Candidat" || currentUser.IsOnboardingCompleted)
            {
                // Si l'utilisateur n'est pas un candidat ou a déjà complété l'onboarding,
                // le rediriger vers son tableau de bord normal.
                return RedirectToAction("DashboardCandidat", "Dashboard");
            }

            // Prépare le ViewModel avec les données existantes pour le pré-remplissage
            var viewModel = new CandidatOnboardingViewModel
            {
                UserId = currentUser.Id,
                Email = currentUser.Email,
                FirstName = currentUser.FirstName,
                LastName = currentUser.LastName,
                PhoneNumber = currentUser.PhoneNumber,
                BirthDate = currentUser.BirthDate,
                // Pré-remplis les champs manquants si des données partielles existent déjà
                Sexe = currentUser.Sexe,
                Nationality = currentUser.Nationality,
                Address = currentUser.Address,
                City = currentUser.City,
                CandidatureCreationDate = currentUser.CreationDate,
                SocialSecurityNumber = currentUser.SocialSecurityNumber,
                FranceTravailNumber = currentUser.FranceTravailNumber,
                ExistingPhotoPath = currentUser.Photo // Affecter le chemin de la photo existante à la nouvelle propriété
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Onboarding(CandidatOnboardingViewModel model)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null || currentUser.Status != "Candidat" || currentUser.IsOnboardingCompleted)
            {
                return RedirectToAction("DashboardCandidat", "Dashboard");
            }

            // Correction ici : utilisez _signInManager.SignOutAsync() sans tenter de capturer la valeur
            if (model.UserId != currentUser.Id)
            {
                await _signInManager.SignOutAsync(); // Déconnecte pour suspicion d'attaque CSRF ou manipulation
                ModelState.AddModelError(string.Empty, "Erreur de sécurité lors de la soumission du formulaire.");
                // Ré-assignez ExistingPhotoPath avant de retourner la vue en cas d'erreur
                model.ExistingPhotoPath = currentUser.Photo;
                return View(model);
            }

            // Assurez-vous que les validations par rapport au PhotoFile ont du sens.
            // Si la photo est requise pour le formulaire d'onboarding initial mais pas pour les mises à jour,
            // vous pourriez avoir besoin d'une logique conditionnelle ici.

            if (ModelState.IsValid)
            {
                // Gérer le téléchargement de la photo
                if (model.PhotoFile != null && model.PhotoFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "images", "profile");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.PhotoFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.PhotoFile.CopyToAsync(fileStream);
                    }

                    currentUser.Photo = "/images/profile/" + uniqueFileName;
                }
                else
                {
                    // Si aucune nouvelle photo n'est téléchargée ET qu'il n'y a PAS de photo existante
                    // alors que le champ est requis (par [Required] sur PhotoFile dans le ViewModel)
                    if (string.IsNullOrEmpty(currentUser.Photo) && model.PhotoFile == null)
                    {
                        ModelState.AddModelError("PhotoFile", "Une photo est requise.");
                        // Ré-assignez ExistingPhotoPath avant de retourner la vue en cas d'erreur
                        model.ExistingPhotoPath = currentUser.Photo;
                        return View(model);
                    }
                    // Si PhotoFile est nul mais qu'il y a déjà une photo existante, ne faites rien.
                    // currentUser.Photo conserve sa valeur actuelle.
                }

                // Mettre à jour les autres propriétés
                currentUser.Sexe = model.Sexe;
                currentUser.Nationality = model.Nationality;
                currentUser.Address = model.Address;
                currentUser.City = model.City;
                currentUser.SocialSecurityNumber = model.SocialSecurityNumber;
                currentUser.FranceTravailNumber = model.FranceTravailNumber;
                // Note : CandidatureCreationDate ne devrait généralement pas être modifiée ici.
                // Si elle l'est, assurez-vous que c'est intentionnel.
                // currentUser.CreationDate = model.CandidatureCreationDate;

                currentUser.IsOnboardingCompleted = true; // Marque l'onboarding comme complété

                var updateResult = await _userManager.UpdateAsync(currentUser);

                if (updateResult.Succeeded)
                {
                    // Redirige vers le tableau de bord du candidat après la complétion
                    return RedirectToAction("DashboardCandidat", "Dashboard");
                }
                else
                {
                    foreach (var error in updateResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            // Si ModelState n'est pas valide ou si la mise à jour a échoué, réaffiche le formulaire avec les erreurs
            // Assurez-vous de toujours repopuler ExistingPhotoPath avant de retourner la vue
            model.ExistingPhotoPath = currentUser.Photo;
            return View(model);
        }
    }
}