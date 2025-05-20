// Controllers/DashboardController.cs
using Microsoft.AspNetCore.Mvc;        // Pour les classes MVC comme Controller, IActionResult
using Microsoft.AspNetCore.Authorization; // Pour l'attribut [Authorize]
using AdminMnsV1.ViewModels;           // Pour utiliser DashboardViewModel
using AdminMnsV1.Interfaces;           // NOUVEAU: Importez l'interface du service que nous allons utiliser
using System.Threading.Tasks;         // Pour les opérations asynchrones
using System;

namespace AdminMnsV1.Controllers
{
    [Authorize] // Attribut: indique que toutes les actions dans ce contrôleur nécessitent une authentification
    public class DashboardController : Controller // Déclaration du contrôleur
    {

        // NOUVEAU: Déclarez une variable pour stocker l'interface de votre service
        // Le contrôleur ne travaille qu'avec l'interface, pas l'implémentation concrète.
        private readonly IDashboardService _dashboardService;

        // Constructeur du contrôleur : ASP.NET Core va injecter l'implémentation de IDashboardService (qui sera DashboardService, grâce à la configuration dans Program.cs)
        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService; // Assigne le service injecté à notre variable privée
        }



        //-------------------------------- Action pour le tableau de bord des Administrateurs---------------------
        [Authorize(Roles = "Admin")] // Attribut: Seuls les utilisateurs avec le rôle "Admin" peuvent accéder à cette action
        public async Task<IActionResult> Dashboard()  // Méthode asynchrone qui retourne un résultat d'action
        {
            ViewData["Title"] = "Tableau de Bord Admin";

            try // Bloque try-catch pour gérer les erreurs
            {
                // Délègue entièrement la récupération des données au service  Le contrôleur appelle simplement une méthode du service et attend le ViewModel.
                var viewModel = await _dashboardService.GetAdminDashboardDataAsync(User);
                return View(viewModel); // Passe le ViewModel à la vue pour affichage
            
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
    }
}