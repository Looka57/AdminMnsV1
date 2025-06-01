// Controllers/CandidaturesStudentsController.cs
using System.Threading.Tasks;
using AdminMnsV1.Application.Services.Interfaces;
using AdminMnsV1.Models.ViewModels; // Pour le ViewModel
using AdminMnsV1.Services.Interfaces; // Pour utiliser ICandidatureService
using Microsoft.AspNetCore.Http; // Pour IFormFile
using Microsoft.AspNetCore.Mvc;


namespace AdminMnsV1.Controllers
{
    public class CandidaturesStudentsController : Controller
    {
        private readonly ICandidatureService _candidatureService;

        public CandidaturesStudentsController(ICandidatureService candidatureService)
        {
            _candidatureService = candidatureService;
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


            return View(viewModel);
        }

        // Action pour la page d'aperçu des candidatures (liste)
        public async Task<IActionResult> Dossiers()
        {
            var viewModels = await _candidatureService.GetAllCandidaturesForOverviewAsync();
            return View(viewModels); // Vous auriez un autre ViewModel pour cette page
        }

        // --- Actions pour les opérations de gestion (exemple) ---

        [HttpPost]
        [ValidateAntiForgeryToken] // Bonnes pratiques de sécurité
        public async Task<IActionResult> UploadDocument(int candidatureId, IFormFile document)
        {
            if (document == null || document.Length == 0)
            {
                TempData["ErrorMessage"] = "Veuillez sélectionner un fichier à télécharger.";
                return RedirectToAction("CandidatureStudent", new { id = candidatureId });
            }

            var success = await _candidatureService.UploadDocumentAsync(candidatureId, document);

            if (!success)
            {
                TempData["ErrorMessage"] = "Erreur lors du téléchargement du document.";
            }
            else
            {
                TempData["SuccessMessage"] = "Document téléchargé avec succès.";
            }
            return RedirectToAction("CandidatureStudent", new { id = candidatureId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ValidateDocument(int documentId)
        {
            var candidatureId = await _candidatureService.ValidateDocumentAsync(documentId);
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
            var candidatureId = await _candidatureService.RejectDocumentAsync(documentId);
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
    }
}