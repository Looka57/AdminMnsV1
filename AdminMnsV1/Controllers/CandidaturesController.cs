using System; // Pour InvalidOperationException et Exception
using System.Collections.Generic;
using System.Linq; // Pour .Select et .ToList()
using System.Threading.Tasks; // Pour Task
using AdminMnsV1.Application.Services.Interfaces; // Utilisez AdminMnsV1.Application.Services.Interfaces pour les services
using AdminMnsV1.Data; // Pour les opérations de base de données asynchrones
using AdminMnsV1.Models; // Assurez-vous d'importer votre modèle Candidature ici
using AdminMnsV1.Models.ViewModels;
using AdminMnsV1.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering; // Pour SelectListItem
// using AdminMnsV1.Services.Interfaces; // Cette ligne semble redondante si les interfaces sont déjà dans Application.Services.Interfaces
using Microsoft.EntityFrameworkCore;
using static AdminMnsV1.Models.ViewModels.CandidaturesOverviewViewModel;

namespace AdminMnsV1.Web.Controllers
{
    [Authorize] // Applique si tu veux que seuls les utilisateurs connectés puissent accéder
    public class CandidaturesController : Controller
    {
        private readonly ICandidatureService _candidatureService;
        private readonly IDocumentTypeService _documentTypeService;
        private readonly IClassService _classService;
        private readonly IDocumentService _documentService;
        private readonly ApplicationDbContext _context; // Assurez-vous d'avoir injecté le DbContext si nécessaire
        private UserManager<User> _userManager;

        public CandidaturesController(
            ICandidatureService candidatureService,
            IDocumentTypeService documentTypeService,
            IClassService classService,
            IDocumentService documentService,
            ApplicationDbContext context,
            UserManager<User> userManager


            )
        {
            _userManager = userManager;
            _candidatureService = candidatureService;
            _documentTypeService = documentTypeService;
            _classService = classService;
            _documentService = documentService;
            _context = context;
        }

        // C'est l'action principale pour afficher la page des "Dossiers" (Candidature.cshtml)
        // Elle doit fournir le CreateCandidatureViewModel pour le modal qui est intégré à cette page.
        [HttpGet]
        public async Task<IActionResult> Candidature()
        {
            // Prépare les données nécessaires pour le modal de création (classes et types de documents)
            var classes = await _classService.GetAllClassesAsync();
            var documentTypes = await _documentTypeService.GetAllDocumentTypesAsync();

            // Correction: Supprime la déclaration redondante et utilise le service pour récupérer toutes les candidatures avec détails.
            var allCandidatures = await _candidatureService.GetAllCandidaturesWithDetailsAsync();

            // 3. Filtre les candidatures par statut
            // Assurez-vous que CandidatureStatus.Label est bien le chemin d'accès au label du statut.
            var candidaturesEnCours = allCandidatures
                .Where(c => c.User != null && c.User.IsDeleted == false && c.CandidatureStatus?.Label?.Equals("En cours", StringComparison.OrdinalIgnoreCase) == true)
                .ToList();

            var candidaturesValidees = allCandidatures
                .Where(c => c.User != null && c.User.IsDeleted == false && c.CandidatureStatus?.Label?.Equals("Validé", StringComparison.OrdinalIgnoreCase) == true)
                .ToList();

            var candidaturesRefusees = allCandidatures
               .Where(c => c.User != null && c.User.IsDeleted == false && c.CandidatureStatus?.Label?.Equals("Refusé", StringComparison.OrdinalIgnoreCase) == true)
                .ToList();

            // ---Calcul des statistiques par classe pour le graphique ---


            var classStats = new List<ClassCandidatureStats>();
            foreach (var classItem in classes)
            {
                var enCoursCount = allCandidatures
                    .Count(c => c.ClassId == classItem.ClasseId &&
                                c.User != null && c.User.IsDeleted == false &&
                                c.CandidatureStatus?.Label?.Equals("En cours", StringComparison.OrdinalIgnoreCase) == true);

                var valideesCount = allCandidatures
                    .Count(c => c.ClassId == classItem.ClasseId &&
                                c.User != null && c.User.IsDeleted == false &&
                                c.CandidatureStatus?.Label?.Equals("Validé", StringComparison.OrdinalIgnoreCase) == true);
                classStats.Add(new ClassCandidatureStats
                {
                    ClassName = classItem.NameClass,
                    EnCoursCount = enCoursCount,
                    ValideesCount = valideesCount
                });
            }






            // 4. Crée le ViewModel combiné : CandidaturesOverviewViewModel
            var viewModel = new CandidaturesOverviewViewModel
            {
                // Propriétés pour la création de candidature (proviennent de l'ancien CreateCandidatureViewModel)
                AvailableClasses = classes.Select(c => new SelectListItem
                {
                    Value = c.ClasseId.ToString(),
                    Text = c.NameClass
                }).ToList(),
                AllAvailableDocumentTypes = documentTypes.ToList(),
                RequiredDocumentTypeIds = new List<int>(), // Initialisation

                // Propriétés pour les listes de candidatures (pour les accordéons)
                // Correction: Le type doit être IEnumerable<Candidature>, pas CandidatureStatus
                CandidaturesEnCours = candidaturesEnCours,
                CandidaturesValidees = candidaturesValidees,
                CandidaturesRefusees = candidaturesRefusees
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCandidature(CandidaturesOverviewViewModel model)
        {
            // Repopule les listes pour le formulaire en cas d'erreur de validation.
            var classes = await _classService.GetAllClassesAsync();
            var documentTypes = await _documentTypeService.GetAllDocumentTypesAsync();
            model.AvailableClasses = classes.Select(c => new SelectListItem
            {
                Value = c.ClasseId.ToString(),
                Text = c.NameClass
            }).ToList();
            model.AllAvailableDocumentTypes = documentTypes.ToList();

            // Populer aussi les listes d'accordéons en cas d'erreur de validation POST !
            var allCandidatures = await _candidatureService.GetAllCandidaturesWithDetailsAsync();
            model.CandidaturesEnCours = allCandidatures
          .Where(c => c.User != null && c.User.IsDeleted == false && c.CandidatureStatus?.Label?.Equals("En cours", StringComparison.OrdinalIgnoreCase) == true)
          .ToList();
            model.CandidaturesValidees = allCandidatures
                .Where(c => c.User != null && c.User.IsDeleted == false && c.CandidatureStatus?.Label?.Equals("Validé", StringComparison.OrdinalIgnoreCase) == true)
                .ToList();
            model.CandidaturesRefusees = allCandidatures
                .Where(c => c.User != null && c.User.IsDeleted == false && c.CandidatureStatus?.Label?.Equals("Refusé", StringComparison.OrdinalIgnoreCase) == true)
                .ToList();

            // --- NOUVEAU : Recalculer les statistiques par classe pour le graphique en cas d'erreur de validation POST ---
            var classStats = new List<ClassCandidatureStats>();
            foreach (var classItem in classes)
            {
                var enCoursCount = allCandidatures
                    .Count(c => c.ClassId == classItem.ClasseId &&
                                c.User != null && c.User.IsDeleted == false &&
                                c.CandidatureStatus?.Label?.Equals("En cours", StringComparison.OrdinalIgnoreCase) == true);

                var valideesCount = allCandidatures
                    .Count(c => c.ClassId == classItem.ClasseId &&
                                c.User != null && c.User.IsDeleted == false &&
                                c.CandidatureStatus?.Label?.Equals("Validé", StringComparison.OrdinalIgnoreCase) == true);

                classStats.Add(new ClassCandidatureStats
                {
                    ClassName = classItem.NameClass,
                    EnCoursCount = enCoursCount,
                    ValideesCount = valideesCount
                });
            }
            model.ClassStats = classStats;



            if (!ModelState.IsValid)
            {
                return View("Candidature", model);
            }

            try
            {
                // CRÉATION ET MAPPAGE DU CreateCandidatureViewModel ICI
                var createCandidatureViewModel = new CreateCandidatureViewModel
                {
                    LastName = model.LastName,
                    FirstName = model.FirstName,
                    Email = model.Email,
                    Phone = model.Phone,
                    BirthDate = model.BirthDate,
                    ClassId = model.ClassId,
                    RequiredDocumentTypeIds = model.RequiredDocumentTypeIds ?? new List<int>()
                    // N'ajoutez pas 'Statut' ici à moins que votre CreateCandidatureViewModel ne l'attende explicitement
                    // Le statut initial (par ex. "En cours") devrait être défini dans le service
                };

                // Passer le ViewModel correctement mappé au service
                var success = await _candidatureService.CreateCandidatureAsync(createCandidatureViewModel);

                if (success)
                {
                    TempData["SuccessMessage"] = "La candidature a été créée avec succès.";
                    return RedirectToAction("Candidature");
                }
                else
                {
                    TempData["ErrorMessage"] = "Une erreur s'est produite lors de la création de la candidature. Veuillez réessayer.";
                    return View("Candidature", model);
                }
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", "Erreur de configuration : " + ex.Message);
                TempData["ErrorMessage"] = "Erreur de configuration : " + ex.Message;
                return View("Candidature", model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Une erreur inattendue est survenue lors de la création du dossier.");
                TempData["ErrorMessage"] = "Une erreur inattendue est survenue : " + ex.Message;
                return View("Candidature", model);
            }
        }
    }
}

// L'action 'Create()' (GET) qui affichait la vue 'Create.cshtml' n'est plus nécessaire.
// Car le modal de création est maintenant intégré directement dans 'Candidature.cshtml',
// et l'action 'Candidature()' (GET) gère son affichage initial.
// Si vous aviez une vue 'Create.cshtml' distincte, elle est maintenant redondante.



//[HttpGet]
//public async Task<IActionResult> Create()
//{
//    var classes = await _classService.GetAllClassesAsync();
//    var documentsTypes = await _documentTypeService.GetAllDocumentTypesAsync();

//    var viewModel = new CreateCandidatureViewModel
//    {
//        AvailableClasses = classes.Select(c => new SelectListItem
//        {
//            Value = c.ClasseId.ToString(),
//            Text = c.NameClass
//        }).ToList(),
//        AllAvailableDocumentTypes = documentsTypes.ToList()
//    };
//    return View(viewModel);
//}
