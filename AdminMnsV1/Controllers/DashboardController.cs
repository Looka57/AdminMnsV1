// Controllers/DashboardController.cs
using Microsoft.AspNetCore.Mvc;        // Pour les classes MVC comme Controller, IActionResult
using Microsoft.AspNetCore.Authorization; // Pour l'attribut [Authorize]
using AdminMnsV1.ViewModels;           // Pour utiliser DashboardViewModel
using System.Threading.Tasks;         // Pour les opérations asynchrones
using System;
using AdminMnsV1.Services.Interfaces;
using AdminMnsV1.Application.Services.Interfaces;
using AdminMnsV1.Application.Services.Implementation;
using System.Security.Claims;

namespace AdminMnsV1.Controllers
{
    [Authorize] // Attribut: indique que toutes les actions dans ce contrôleur nécessitent une authentification
    public class DashboardController : Controller // Déclaration du contrôleu
    {

        // NOUVEAU: Déclarez une variable pour stocker l'interface de votre service
        // Le contrôleur ne travaille qu'avec l'interface, pas l'implémentation concrète.
        private readonly IDashboardService _dashboardService;
        private readonly ICandidatureService _candidatureService; // Injectez ce service


        // Constructeur du contrôleur : ASP.NET Core va injecter l'implémentation de IDashboardService (qui sera DashboardService, grâce à la configuration dans Program.cs)
        public DashboardController(IDashboardService dashboardService, ICandidatureService candidatureService)
        {
            _dashboardService = dashboardService; // Assigne le service injecté à notre variable privée
            _candidatureService = candidatureService; // <<< Assigner le service injecté à la variable privée


        }



        //-------------------------------- Action pour le tableau de bord des Administrateurs---------------------
        [Authorize(Roles = "Admin")] // Attribut: Seuls les utilisateurs avec le rôle "Admin" peuvent accéder à cette action
        public async Task<IActionResult> Dashboard()  // Méthode asynchrone qui retourne un résultat d'action
        {
            ViewData["Title"] = "Tableau de Bord Admin";

            try
            {
                // Délègue la récupération des données au service pour les données principales du tableau de bord
                var viewModel = await _dashboardService.GetAdminDashboardDataAsync(User);

                // NOUVEAU : Récupérer les 5 dernières candidatures par date de création
                var latestCandidatures = await _candidatureService.GetLatestCandidaturesByCreationDateAsync(5);

                // Assigner la liste des dernières candidatures au ViewModel
                viewModel.LatestViewedCandidatures = latestCandidatures.ToList();

                return View(viewModel);
            }

            catch (UnauthorizedAccessException) // Si le service lève cette exception (utilisateur non trouvé/autorisé)
            {
                return RedirectToAction("Login", "Home"); // Le contrôleur décide de rediriger vers la page de connexion
            }
            catch (Exception ex) // Pour toutes les autres exceptions inattendues
            {
                // Gérer d'autres erreurs potentielles (logging, page d'erreur générique)
                return StatusCode(500, "Une erreur interne est survenue lors du chargement du tableau de bord.");  // Retourne un code d'erreur HTTP 500 avec un message convivial
            }
        }

        //-------------------------------- Action pour le tableau de bord des Students--------------------
        [Authorize(Roles = "Student")] // Attribut: Seuls les utilisateurs avec le rôle "Student" peuvent accéder à cette action
        public async Task<IActionResult> DashboardStudent()
        {
            ViewData["Title"] = "Tableau de Bord Student";

            try
            {
                // Délègue entièrement la récupération des données au service
                var viewModel = await _dashboardService.GetStudentDashboardDataAsync(User);
                return View(viewModel);
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("Login", "Home");
            }
            catch (Exception ex)
            {
                // Console.WriteLine(ex.Message);
                return StatusCode(500, "Une erreur interne est survenue lors du chargement du tableau de bord du stagiaire.");
            }
        }



        [Authorize(Roles = "Student")]
        public async Task<IActionResult> DashboardCandidat()
        {
            ViewData["Title"] = "Tableau de Bord Candidat";

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var candidature = await _candidatureService.GetCandidatureByUserIdAsync(userId); // Ceci va maintenant fonctionner

                DashboardViewModel viewModel;

                if (candidature != null)
                {
                    // Si le label est "En cours" (ou "Candidat" si vous avez les deux statuts distincts)
                    if (candidature.CandidatureStatus?.Label == "En cours" || candidature.CandidatureStatus?.Label == "Candidat")
                    {
                        viewModel = await _dashboardService.GetCandidatDashboardDataAsync(User); // Ceci doit donner 2 cartes
                    }
                    else
                    {
                        viewModel = await _dashboardService.GetStudentDashboardDataAsync(User); // Ceci donne 3 cartes
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Vous devez d'abord créer votre dossier de candidature pour accéder à ce tableau de bord.";
                    return RedirectToAction("Create", "Candidature");
                }

                return View(viewModel);
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }
            catch (Exception ex)
            {
                // Considérez d'utiliser un ILogger pour logger cette exception.
                // Console.WriteLine(ex.Message); // Bon pour le débogage, moins pour la production.
                return StatusCode(500, "Une erreur interne est survenue lors du chargement du tableau de bord du stagiaire.");
            }


        }
    }


}
