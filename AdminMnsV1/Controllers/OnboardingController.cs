using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using AdminMnsV1.Models;
using AdminMnsV1.Models.ViewModels; // Ou l'emplacement correct de CandidatOnboardingViewModel
using System.Threading.Tasks;
using AdminMnsV1.Interfaces.IServices;
using AdminMnsV1.ViewModels; // Importez votre interface de service

namespace AdminMnsV1.Controllers
{
    [Authorize(Roles = "Student")]
    public class OnboardingController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IOnboardingService _onboardingService; // <--- Nouvelle injection

        public OnboardingController(UserManager<User> userManager, SignInManager<User> signInManager, IOnboardingService onboardingService) // <--- Modifiez le constructeur
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _onboardingService = onboardingService; // <--- Initialisation du service
        }

        [HttpGet]
        public async Task<IActionResult> Onboarding()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null || currentUser.Status != "Candidat" || currentUser.IsOnboardingCompleted)
            {
                return RedirectToAction("DashboardCandidat", "Dashboard");
            }

            var viewModel = new CandidatOnboardingViewModel
            {
                UserId = currentUser.Id,
                Email = currentUser.Email,
                FirstName = currentUser.FirstName,
                LastName = currentUser.LastName,
                PhoneNumber = currentUser.PhoneNumber,
                BirthDate = currentUser.BirthDate,
                Sexe = currentUser.Sexe,
                Nationality = currentUser.Nationality,
                Address = currentUser.Address,
                City = currentUser.City,
                CandidatureCreationDate = currentUser.CreationDate,
                SocialSecurityNumber = currentUser.SocialSecurityNumber,
                FranceTravailNumber = currentUser.FranceTravailNumber,
                ExistingPhotoPath = currentUser.Photo
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

            if (model.UserId != currentUser.Id)
            {
                await _signInManager.SignOutAsync();
                ModelState.AddModelError(string.Empty, "Erreur de sécurité lors de la soumission du formulaire.");
                model.ExistingPhotoPath = currentUser.Photo;
                return View(model);
            }

            // La validation de ModelState.IsValid doit rester ici pour les validations de base du ViewModel
            if (!ModelState.IsValid)
            {
                // Si le modèle n'est pas valide, repopuler ExistingPhotoPath avant de retourner la vue
                model.ExistingPhotoPath = currentUser.Photo;
                return View(model);
            }

            // *** Appel au service pour toute la logique d'onboarding ***
            var (succeeded, errorMessage) = await _onboardingService.ProcessOnboardingAsync(currentUser, model, model.PhotoFile);

            if (succeeded)
            {
                return RedirectToAction("DashboardCandidat", "Dashboard");
            }
            else
            {
                ModelState.AddModelError(string.Empty, errorMessage);
                // Repopuler ExistingPhotoPath en cas d'erreur du service
                model.ExistingPhotoPath = currentUser.Photo;
                return View(model);
            }
        }
    }
}