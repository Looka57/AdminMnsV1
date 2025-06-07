// Dans votre fichier AdminController.cs (ou le nom de votre contrôleur)
using Microsoft.AspNetCore.Mvc;
using AdminMnsV1.Application.Services.Interfaces; // Assurez-vous que le chemin est correct
using System.Threading.Tasks;

public class AdminController : Controller
{
    private readonly ICandidatureService _candidatureService;

    // Assurez-vous que ICandidatureService est bien injecté
    public AdminController(ICandidatureService candidatureService)
    {
        _candidatureService = candidatureService;
    }

    // Action pour afficher les détails d'une candidature (celle que vous utilisez pour cette vue)
    public async Task<IActionResult> CandidatureDetails(int id)
    {
        var model = await _candidatureService.GetCandidatureDetailsAsync(id);
        if (model == null)
        {
            return NotFound(); // Gérer le cas où la candidature n'existe pas
        }
        return View(model);
    }

    [HttpPost] // Cette action sera appelée via un formulaire POST
    public async Task<IActionResult> ValidateDocument(int documentId)
    {
        // Appelle le service pour valider le document et obtenir l'ID de la candidature
        var candidatureId = await _candidatureService.ValidateDocumentAsync(documentId);

        if (candidatureId > 0)
        {
            TempData["SuccessMessage"] = "Document validé avec succès.";
            // Redirige vers la page de détails de la candidature pour rafraîchir
            return RedirectToAction("CandidatureDetails", new { id = candidatureId });
        }
        else
        {
            TempData["ErrorMessage"] = "Erreur: Document non trouvé ou validation impossible.";
            return RedirectToAction("CandidatureDetails", new { id = TempData["LastCandidatureId"] }); // Tente de revenir à la dernière page connue
        }
    }

    [HttpPost] // Cette action sera appelée via un formulaire POST
    public async Task<IActionResult> RejectDocument(int documentId)
    {
        // Appelle le service pour rejeter le document et obtenir l'ID de la candidature
        var candidatureId = await _candidatureService.RejectDocumentAsync(documentId);

        if (candidatureId > 0)
        {
            TempData["SuccessMessage"] = "Document rejeté avec succès.";
            // Redirige vers la page de détails de la candidature pour rafraîchir
            return RedirectToAction("CandidatureDetails", new { id = candidatureId });
        }
        else
        {
            TempData["ErrorMessage"] = "Erreur: Document non trouvé ou rejet impossible.";
            return RedirectToAction("CandidatureDetails", new { id = TempData["LastCandidatureId"] });
        }
    }

    // Optionnel : Action pour supprimer une candidature (si le bouton est activé)
    [HttpPost]
    public async Task<IActionResult> DeleteCandidature(int id)
    {
        var result = await _candidatureService.DeleteCandidatureAsync(id);
        if (result)
        {
            TempData["SuccessMessage"] = "Candidature supprimée avec succès.";
            return RedirectToAction("AllCandidaturesOverview"); // Rediriger vers la liste des candidatures
        }
        else
        {
            TempData["ErrorMessage"] = "Erreur lors de la suppression de la candidature.";
            return RedirectToAction("CandidatureDetails", new { id = id });
        }
    }

    // N'oubliez pas l'action pour la liste de toutes les candidatures (si elle existe)
    public async Task<IActionResult> AllCandidaturesOverview()
    {
        var model = await _candidatureService.GetAllCandidaturesForOverviewAsync();
        return View(model);
    }
}