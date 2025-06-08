// Controllers/CandidaturesStudentsController.cs
using System.Security.Claims;
using System.Threading.Tasks;
using AdminMnsV1.Application.Services.Interfaces;
using AdminMnsV1.Models;
using AdminMnsV1.Models.CandidaturesModels;
using AdminMnsV1.Models.ViewModels; // Pour le ViewModel
using AdminMnsV1.Services.Interfaces; // Pour utiliser ICandidatureService
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http; // Pour IFormFile
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


namespace AdminMnsV1.Controllers
{
    public class CandidaturesStudentsController : Controller
    {
        private readonly ICandidatureService _candidatureService;
        private string documentTypeName;
        private readonly UserManager<User> _userManager;


        public CandidaturesStudentsController(ICandidatureService candidatureService, UserManager<User> userManager)
        {
            _candidatureService = candidatureService;
            _userManager = userManager;

        }

        // Action pour afficher les détails d'une candidature
        public async Task<IActionResult> CandidatureStudent(int id)
        {
            var viewModel = await _candidatureService.GetCandidatureDetailsAsync(id);

            if (viewModel == null)
            {
                return NotFound(); // Retourne une page 404 si la candidature n'est pas trouvée
            }

            // Définir le titre de la page pour la vue
            // Dans votre contrôleur, là où vous préparez le ViewData["Title"]
            string studentFullName = $"{viewModel.FirstName ?? ""} {(viewModel.LastName ?? "")}".Trim();

            // Vérifiez si le nom complet est vide ou juste des espaces après le trim
            if (string.IsNullOrWhiteSpace(studentFullName))
            {
                ViewData["Title"] = "Dossier: Candidat Inconnu";
            }
            else
            {
                ViewData["Title"] = "Dossier: " + studentFullName;
            }


            return View("CandidatureStudent", CandidatureStudent);
        }

        // Action pour la page d'aperçu des candidatures (liste)
        public async Task<IActionResult> Dossiers()
        {
            var viewModels = await _candidatureService.GetAllCandidaturesForOverviewAsync();
            return View(viewModels); // Vous auriez un autre ViewModel pour cette page
        }





        // --- Actions pour les opérations de gestion ---

        [HttpPost]
        [ValidateAntiForgeryToken] // Bonnes pratiques de sécurité

        public async Task<IActionResult> UploadDocument(int candidatureId, IFormFile document, string documentTypeName)
        {
            if (document == null || document.Length == 0)
            {
                TempData["ErrorMessage"] = "Veuillez sélectionner un fichier à télécharger.";
                return DetermineRedirectAction(candidatureId);
            }

            // L'action C# doit recevoir le documentTypeName
            var success = await _candidatureService.UploadDocumentAsync(candidatureId, document, documentTypeName);

            if (!success)
            {
                TempData["ErrorMessage"] = "Erreur lors du téléchargement du document.";
            }
            else
            {
                TempData["SuccessMessage"] = "Document téléchargé avec succès.";
            }
            return RedirectToAction("Mydossier", "CandidaturesStudents");
        }

        private IActionResult DetermineRedirectAction(int candidatureId)
        {
            // POINT DE DÉBOGAGE CRITIQUE !
            // Placez un point d'arrêt ici.
            // Examinez User.IsInRole("Student") et User.IsInRole("Admin")
            // Tracez le flux d'exécution pas à pas.

            if (User.IsInRole("Student")) // <-- Est-ce que cette condition est VRAIE ici ?
            {
                return RedirectToAction("MyDossier");
            }
            else if (User.IsInRole("Admin") || User.IsInRole("MNS")) // <-- Si "Student" est faux, est-ce que celle-ci est VRAIE ?
            {
                return RedirectToAction("CandidatureStudent", new { id = candidatureId });
            }
            // Alternative si "candidat" n'est PAS un rôle mais un claim personnalisé
            else if (User.HasClaim(c => c.Type == "StatutUtilisateur" && c.Value == "candidat")) // <-- Et celle-ci ?
            {
                return RedirectToAction("MyDossier");
            }
            else
            {
                // Fallback
                return RedirectToAction("CandidatureStudent", "Home"); // Votre route de fallback
            }
        }












        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ValidateDocument(int documentId)
        {
            var adminUserId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(adminUserId))
            {
                TempData["ErrorMessage"] = "Opération non autorisée. L'utilisateur n'est pas connecté.";
                return Unauthorized(); // Ou un autre RedirectToAction approprié
            }

            var candidatureId = await _candidatureService.ValidateDocumentAsync(documentId, adminUserId);
            if (candidatureId == 0)
            {
                return NotFound(); // Document ou candidature non trouvé
            }
            TempData["SuccessMessage"] = "Document validé avec succès.";
            return RedirectToAction("CandidatureStudent", new { id = candidatureId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectDocument(int documentId)
        {
            // 1. Récupérer l'ID de l'utilisateur actuellement connecté (l'admin)
            var adminUserId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(adminUserId))
            {
                TempData["ErrorMessage"] = "Opération non autorisée. L'utilisateur n'est pas connecté.";
                return Unauthorized(); // Ou un autre RedirectToAction approprié
            }

            var candidatureId = await _candidatureService.RejectDocumentAsync(documentId, adminUserId);
            if (candidatureId == 0)
            {
                return NotFound();
            }
            TempData["SuccessMessage"] = "Document rejeté.";
            return RedirectToAction("CandidatureStudent", new { id = candidatureId });
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCandidature(int id)
        {
            var success = await _candidatureService.DeleteCandidatureAsync(id);
            if (!success)
            {
                TempData["ErrorMessage"] = "Erreur lors de la suppression du dossier.";
                return RedirectToAction("CandidatureStudent", new { id = id });
            }
            TempData["SuccessMessage"] = "Dossier supprimé avec succès.";
            return RedirectToAction("Dossiers"); // Redirige vers la liste des dossiers après suppression
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int candidatureId, string newStatus)
        {
            var success = await _candidatureService.UpdateCandidatureStatusAsync(candidatureId, newStatus);
            if (!success)
            {
                TempData["ErrorMessage"] = "Erreur lors de la mise à jour du statut.";
                return RedirectToAction("CandidatureStudent", new { id = candidatureId });
            }
            TempData["SuccessMessage"] = $"Statut mis à jour à '{newStatus}'.";
            return RedirectToAction("CandidatureStudent", new { id = candidatureId });
        }




        //Afficher le dossier du candidat connecté

        public async Task<IActionResult> MyDossier()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToPage("Home/Login");
            }

            // Récupère le dossier de candidature de l'utilisateur via le CandidatureService

            var candidatureViewModel = await _candidatureService.GetCandidatureDetailsByUserIdAsync(userId);

            if (candidatureViewModel == null)
            {
                TempData["ErrorMessage"] = "Aucun dossier de candidature trouvé pour votre compte.";
                // Vous pouvez créer une vue DossierNotFound.cshtml pour ce cas
                return View("~/Views/Students/DossierNotFound.cshtml"); // Redirige vers une page d'erreur
            }

            return View("~/Views/CandidaturesStudents/CandidaturesStudentsCandidat.cshtml", candidatureViewModel); // Assurez-vous que cette vue est créée (étape 3)

        }
    }
}

