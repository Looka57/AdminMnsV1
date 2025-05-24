using Microsoft.AspNetCore.Mvc;
using AdminMnsV1.Application.Services.Interfaces; // Utilisez AdminMnsV1.Application.Services.Interfaces pour les services
using AdminMnsV1.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks; // Pour Task
using Microsoft.AspNetCore.Mvc.Rendering; // Pour SelectListItem
using System.Linq; // Pour .Select et .ToList()
using System; // Pour InvalidOperationException et Exception
using System.Collections.Generic;
using AdminMnsV1.Services.Interfaces; // Pour List<int>

namespace AdminMnsV1.Web.Controllers
{
    [Authorize] // Applique si tu veux que seuls les utilisateurs connectés puissent accéder
    public class CandidaturesController : Controller
    {
        private readonly ICandidatureService _candidatureService;
        private readonly IDocumentTypeService _documentTypeService;
        private readonly IClassService _classService;
        private readonly IDocumentService _documentService;

        public CandidaturesController(
            ICandidatureService candidatureService,
            IDocumentTypeService documentTypeService,
            IClassService classService,
            IDocumentService documentService)
        {
            _candidatureService = candidatureService;
            _documentTypeService = documentTypeService;
            _classService = classService;
            _documentService = documentService;
        }

        // C'est l'action principale pour afficher la page des "Dossiers" (Candidature.cshtml)
        // Elle doit fournir le CreateCandidatureViewModel pour le modal qui est intégré à cette page.
        [HttpGet]
        public async Task<IActionResult> Candidature()
        {
            // Prépare les données nécessaires pour le modal de création (classes et types de documents)
            var classes = await _classService.GetAllClassesAsync();
            var documentTypes = await _documentTypeService.GetAllDocumentTypesAsync();

            var viewModel = new CreateCandidatureViewModel
            {
                AvailableClasses = classes.Select(c => new SelectListItem
                {
                    Value = c.ClasseId.ToString(),
                    Text = c.NameClass
                }).ToList(),
                AllAvailableDocumentTypes = documentTypes.ToList(),
                // Initialise SelectedDocumentTypeIds pour éviter NullReferenceException dans la vue
                SelectedDocumentTypeIds = new List<int>()
            };

            // Si vous avez d'autres listes (ex: candidatures en cours, validées) à afficher
            // dans les accordéons de Candidature.cshtml, vous devriez les charger ici
            // et les ajouter à ce viewModel (ou à un viewModel plus complexe si nécessaire).
            // Pour l'instant, on se concentre sur le fonctionnement du modal.

            return View(viewModel); // Passe l'instance du ViewModel à la vue Candidature.cshtml
        }

        // Cette action est appelée quand le formulaire de création de candidature est soumis (POST)
        [HttpPost]
        [ValidateAntiForgeryToken] // Pour la sécurité CSRF
        public async Task<IActionResult> CreateCandidature(CreateCandidatureViewModel model)
        {
            // Repopule les listes (classes, types de documents) AVANT de vérifier ModelState.IsValid,
            // car elles sont nécessaires pour réafficher le formulaire (le modal) en cas d'erreur de validation.
            var classes = await _classService.GetAllClassesAsync();
            var documentTypes = await _documentTypeService.GetAllDocumentTypesAsync();
            model.AvailableClasses = classes.Select(c => new SelectListItem
            {
                Value = c.ClasseId.ToString(),
                Text = c.NameClass
            }).ToList();
            model.AllAvailableDocumentTypes = documentTypes.ToList();

            if (!ModelState.IsValid)
            {
                // Si la validation échoue, retourner la vue Candidature pour réafficher le modal avec les erreurs.
                // Le modèle "model" contient les données soumises et les messages d'erreur.
                return View("Candidature", model);
            }

            try
            {
                // Appelle le service pour exécuter la logique métier de création de la candidature
                var success = await _candidatureService.CreateCandidatureAsync(model);

                if (success)
                {
                    TempData["SuccessMessage"] = "La candidature a été créée avec succès.";
                    // Redirige vers l'action GET Candidature() pour recharger la page principale.
                    // Cela permet de vider le formulaire et d'afficher le message de succès.
                    return RedirectToAction("Candidature");
                }
                else
                {
                    TempData["ErrorMessage"] = "Une erreur s'est produite lors de la création de la candidature. Veuillez réessayer.";
                    // Retourner la vue Candidature avec le modèle pour afficher l'erreur.
                    return View("Candidature", model);
                }
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", "Erreur de configuration : " + ex.Message);
                TempData["ErrorMessage"] = "Erreur de configuration : " + ex.Message;
                // Retourner la vue Candidature avec le modèle pour afficher l'erreur.
                return View("Candidature", model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Une erreur inattendue est survenue lors de la création du dossier.");
                TempData["ErrorMessage"] = "Une erreur inattendue est survenue : " + ex.Message;
                // Retourner la vue Candidature avec le modèle pour afficher l'erreur.
                return View("Candidature", model);
            }
        }

        // L'action 'Create()' (GET) qui affichait la vue 'Create.cshtml' n'est plus nécessaire.
        // Car le modal de création est maintenant intégré directement dans 'Candidature.cshtml',
        // et l'action 'Candidature()' (GET) gère son affichage initial.
        // Si vous aviez une vue 'Create.cshtml' distincte, elle est maintenant redondante.
        /*
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var classes = await _classService.GetAllClassesAsync();
            var documentsTypes = await _documentTypeService.GetAllDocumentTypesAsync();

            var viewModel = new CreateCandidatureViewModel
            {
                AvailableClasses = classes.Select(c => new SelectListItem
                {
                    Value = c.ClasseId.ToString(),
                    Text = c.NameClass
                }).ToList(),
                AllAvailableDocumentTypes = documentsTypes.ToList()
            };
            return View(viewModel);
        }
        */
    }
}