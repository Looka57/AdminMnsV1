// Services/DashboardService.cs
using AdminMnsV1.Data;         // Pour interagir avec la base de données (ApplicationDbContext)
using AdminMnsV1.Interfaces;   // Pour implémenter notre interface IDashboardService
using AdminMnsV1.Models;      // Pour utiliser des modèles comme User et CardModel
using AdminMnsV1.ViewModels;  // Pour construire le DashboardViewModel
using Microsoft.AspNetCore.Identity; // Pour utiliser UserManager (gestion des utilisateurs et de leurs rôles)
using System.Collections.Generic; // Pour utiliser List<>
using System.Linq;            // Pour les requêtes LINQ (Count(), Where())
using System.Security.Claims; // Pour ClaimsPrincipal
using System.Threading.Tasks; // Pour les opérations asynchrones
using System;


//Maintenant que nous avons défini le contrat avec l'interface, nous devons créer une classe qui va "réaliser" ce contrat. C'est la classe DashboardService qui implémente IDashboardService

namespace AdminMnsV1.Services.Implementation
{
    public class DashboardService : IDashboardService //implémente 'IDashboardService'
    {
        private readonly ApplicationDbContext _context; // Variable pour accéder à la base de données
        private readonly UserManager<User> _userManager; // Variable pour gérer les utilisateurs




        // Le service injecte le DbContext et l'UserManager
        // Constructeur du service : C'est ici que le service reçoit ses outils (dépendances)
        // ASP.NET Core "injectera" automatiquement ces objets quand il créera une instance de DashboardService
        public DashboardService(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;  // Assigne le DbContext injecté à notre variable privée
            _userManager = userManager; // Assigne le UserManager injecté à notre variable privée
        }


        // -------------Implémentation de la méthode du tableau de bord Admin------------
        public async Task<DashboardViewModel> GetAdminDashboardDataAsync(ClaimsPrincipal userPrincipal)
        {

             //Récupérer les informations détaillées de l'utilisateur connecté
            var currentUser = await _userManager.GetUserAsync(userPrincipal); // 'userPrincipal' contient des informations de base (ID, nom, etc.), mais 'GetUserAsync' récupère l'objet 'User' complet de la base de données.

            if (currentUser == null) //Si l'utilisateur n'est pas trouvé (ce qui est rare avec [Authorize])
            {
                // Un service ne doit PAS rediriger. Il doit informer l'appelant (ici, le contrôleur)
                // qu'une erreur s'est produite. Lever une exception est la meilleure pratique ici.
                throw new UnauthorizedAccessException("Utilisateur non trouvé ou non autorisé.");
            }

            // Récupérer toutes les données statistiques nécessaires au tableau de bord Admin
            // Cette logique était auparavant dans le contrôleur. Elle est maintenant ici,car c'est la responsabilité du service de préparer les données. 

            var classCount = _context.SchoolClass.Count(); // Compte le nombre total de classes
            var studentCount = _context.Users // Filtre les utilisateurs par statut "Stagiaire" et non supprimés
                .Where(u => u.Status == "Stagiaire" && !u.IsDeleted)
                .Count();
            var numberMen = _context.Users // Compte les hommes stagiaires
                .Where(u => u.Status == "Stagiaire" && u.Sexe == "Male")
                .Count();
            var numberWomen = _context.Users // Compte les femmes stagiaires
                .Where(u => u.Status == "Stagiaire" && u.Sexe == "Female")
                .Count();



            // Construire le ViewModel final qui sera envoyé à la vue
            // Le ViewModel regroupe toutes les données nécessaires à l'affichage du tableau de bord.
            var cards = new List<CardModel>
            {
                new CardModel { Url = "../Classes/Class", Number = classCount.ToString(), Title = "Classes", IconUrl = "https://img.icons8.com/glyph-neue/64/classroom.png", AltText = "classroom" },
                new CardModel { Url = "../Candidatures/Candidature", Number = "254", Title = "Dossiers", IconUrl = "https://img.icons8.com/glyph-neue/64/user-folder.png", AltText = "user-folder" },
                new CardModel { Url = "../Students/Student", Number = studentCount.ToString(), Title = "Stagiaires", IconUrl = "https://img.icons8.com/glyph-neue/64/student-male.png", AltText = "student-male" },
                new CardModel { Url = "#", Number = "5", Title = "Notifications", IconUrl = "https://img.icons8.com/ios-filled/50/appointment-reminders--v1.png", AltText = "reminder" }
            };

            // Créer et peupler votre DashboardViewModel
            // Construire le ViewModel final qui sera envoyé à la vue. Le ViewModel regroupe toutes les données nécessaires à l'affichage du tableau de bord.
            var viewModel = new DashboardViewModel
            {
                LoggedInUser = currentUser, // L'utilisateur connecté pour afficher son nom
                Cards = cards, // La liste des cartes d'informations
                TotalClasses = classCount, // Les statistiques pour les graphiques/autres affichages
                TotalStudents = studentCount,
                NumberOfMen = numberMen,
                NumberOfWomen = numberWomen
            };

            return viewModel;
        }

        // -------------Implémentation de la méthode du tableau de bord Stagiaire------------
        public async Task<DashboardViewModel> GetStudentDashboardDataAsync(ClaimsPrincipal userPrincipal)
        {
            var currentUser = await _userManager.GetUserAsync(userPrincipal);
            if (currentUser == null)
            {
                throw new UnauthorizedAccessException("Utilisateur non trouvé ou non autorisé.");
            }

            var cards = new List<CardModel>
            {
                new CardModel { Url = "../Classes/Class", Number = "5", Title = "Classes", IconUrl = "https://img.icons8.com/glyph-neue/64/classroom.png", AltText = "classroom" },
                new CardModel { Url = "../Candidatures/Candidature", Number = "1", Title = "Dossiers", IconUrl = "https://img.icons8.com/glyph-neue/64/user-folder.png", AltText = "user-folder" },
                new CardModel { Url = "#", Number = "5", Title = "Notifications", IconUrl = "https://img.icons8.com/ios-filled/50/appointment-reminders--v1.png", AltText = "reminder" }
            };

            var viewModel = new DashboardViewModel
            {
                LoggedInUser = currentUser,
                Cards = cards,
                // Ajoutez ici les données spécifiques au tableau de bord du stagiaire si nécessaire
            };

            return viewModel;
        }
    }
}