using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdminMnsV1.Application.Services.Interfaces;
using AdminMnsV1.Data;
using AdminMnsV1.Models; // Assurez-vous d'importer votre modèle Candidature ici
using AdminMnsV1.Models.ViewModels;
using AdminMnsV1.Services.Interfaces; // Cette ligne semble redondante si les interfaces sont déjà dans Application.Services.Interfaces
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using static AdminMnsV1.Models.ViewModels.CandidaturesOverviewViewModel;

namespace AdminMnsV1.Web.Controllers
{
    [Authorize]
    public class CandidaturesController : Controller
    {
        private readonly ICandidatureService _candidatureService;
        private readonly IDocumentTypeService _documentTypeService;
        private readonly IClassService _classService;
        private readonly IDocumentService _documentService;
        private readonly ApplicationDbContext _context;
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

        [HttpGet]
        public async Task<IActionResult> Candidature()
        {
            var classes = await _classService.GetAllClassesAsync();
            var documentTypes = await _documentTypeService.GetAllDocumentTypesAsync();

            // S'assurer que le service retourne les candidatures avec leurs statuts inclus (via .Include)
            var allCandidatures = await _candidatureService.GetAllCandidaturesWithDetailsAsync();

            // 3. Filtre les candidatures par statut pour les listes d'accordéons
            // CORRECTION ICI : Utiliser c.CandidatureStatus?.Label
            var candidaturesEnCours = allCandidatures
                .Where(c => c.User != null && c.User.IsDeleted == false && c.CandidatureStatuses?.Label?.Equals("En cours", StringComparison.OrdinalIgnoreCase) == true)
                .ToList();

            var candidaturesValidees = allCandidatures
                .Where(c => c.User != null && c.User.IsDeleted == false && c.CandidatureStatuses?.Label?.Equals("Validé", StringComparison.OrdinalIgnoreCase) == true)
                .ToList();

            var candidaturesRefusees = allCandidatures
               .Where(c => c.User != null && c.User.IsDeleted == false &&
                           (c.CandidatureStatuses?.Label?.Equals("Refusé", StringComparison.OrdinalIgnoreCase) == true ||
                            c.CandidatureStatuses?.Label?.Equals("Supprimé", StringComparison.OrdinalIgnoreCase) == true ||
                            c.CandidatureStatuses?.Label?.Equals("Invalidé", StringComparison.OrdinalIgnoreCase) == true)) // Correction pour inclure tous les statuts "terminés"
                .ToList();

            // ---Calcul des statistiques par classe pour le graphique ---
            var classStats = new List<ClassCandidatureStats>();
            foreach (var classItem in classes)
            {
                var enCoursCount = allCandidatures
                    .Count(c => c.ClassId == classItem.ClasseId &&
                                 c.User != null && c.User.IsDeleted == false &&
                                 c.CandidatureStatuses?.Label?.Equals("En cours", StringComparison.OrdinalIgnoreCase) == true);

                // IMPORTANT : Si "Validé" seul ne capture pas tous vos "dossiers clôturés",
                // vous devrez ajouter d'autres libellés ici (ex: "Refusé", "Supprimé", "Invalidé").
                // D'après votre description, il est probable que "Validé" soit juste un statut parmi d'autres qui indiquent un dossier "clôturé" pour le graphique.
                // Si "Validé" est le seul statut qui compte comme "Validé/Clôturé" sur le graphique, alors c'est bon.
                var valideesCount = allCandidatures
                    .Count(c => c.ClassId == classItem.ClasseId &&
                                 c.User != null && c.User.IsDeleted == false &&
                                 c.CandidatureStatuses?.Label?.Equals("Validé", StringComparison.OrdinalIgnoreCase) == true);

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
                // Propriétés pour la création de candidature
                AvailableClasses = classes.Select(c => new SelectListItem
                {
                    Value = c.ClasseId.ToString(),
                    Text = c.NameClass
                }).ToList(),
                AllAvailableDocumentTypes = documentTypes.ToList(),
                RequiredDocumentTypeIds = new List<int>(), // Initialisation

                // Propriétés pour les listes de candidatures (pour les accordéons)
                // CES LIGNES ONT ÉTÉ CORRIGÉES
                CandidaturesEnCours = candidaturesEnCours, // Utilisation des listes déjà filtrées
                CandidaturesValidees = candidaturesValidees,
                CandidaturesRefusees = candidaturesRefusees,

                // Assurez-vous que la propriété ClassStats est assignée ici pour le graphique
                ClassStats = classStats
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
            // Ces parties sont déjà correctement corrigées dans votre code actuel
            var allCandidatures = await _candidatureService.GetAllCandidaturesWithDetailsAsync();
            model.CandidaturesEnCours = allCandidatures
            .Where(c => c.User != null && c.User.IsDeleted == false && c.CandidatureStatuses?.Label?.Equals("En cours", StringComparison.OrdinalIgnoreCase) == true)
            .ToList();
            model.CandidaturesValidees = allCandidatures
                .Where(c => c.User != null && c.User.IsDeleted == false && c.CandidatureStatuses?.Label?.Equals("Validé", StringComparison.OrdinalIgnoreCase) == true)
                .ToList();
            model.CandidaturesRefusees = allCandidatures
                .Where(c => c.User != null && c.User.IsDeleted == false &&
                            (c.CandidatureStatuses?.Label?.Equals("Refusé", StringComparison.OrdinalIgnoreCase) == true ||
                             c.CandidatureStatuses?.Label?.Equals("Supprimé", StringComparison.OrdinalIgnoreCase) == true ||
                             c.CandidatureStatuses?.Label?.Equals("Invalidé", StringComparison.OrdinalIgnoreCase) == true))
                .ToList();

            // Recalculer les statistiques par classe pour le graphique en cas d'erreur de validation POST
            var classStats = new List<ClassCandidatureStats>();
            foreach (var classItem in classes)
            {
                var enCoursCount = allCandidatures
                    .Count(c => c.ClassId == classItem.ClasseId &&
                                 c.User != null && c.User.IsDeleted == false &&
                                 c.CandidatureStatuses?.Label?.Equals("En cours", StringComparison.OrdinalIgnoreCase) == true);

                var valideesCount = allCandidatures
                    .Count(c => c.ClassId == classItem.ClasseId &&
                                 c.User != null && c.User.IsDeleted == false &&
                                 c.CandidatureStatuses?.Label?.Equals("Validé", StringComparison.OrdinalIgnoreCase) == true);

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
                var createCandidatureViewModel = new CreateCandidatureViewModel
                {
                    LastName = model.LastName,
                    FirstName = model.FirstName,
                    Email = model.Email,
                    Phone = model.Phone,
                    BirthDate = model.BirthDate,
                    ClassId = model.ClassId,
                    RequiredDocumentTypeIds = model.RequiredDocumentTypeIds ?? new List<int>()
                };

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