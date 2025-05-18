using AdminMnsV1.Data;
using AdminMnsV1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AdminMnsV1.ViewModels; // Assurez-vous d'avoir ce 'using'
using System.Threading.Tasks;
using System.Linq; // Pour les méthodes Count(), Where()

namespace AdminMnsV1.Controllers
{
    [Authorize] // S'applique à toutes les actions du contrôleur par défaut
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public DashboardController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Tableau de Bord pour les ADMINS (une seule action désormais)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Dashboard() // Renommé en "Dashboard" pour plus de clarté
        {
            ViewData["Title"] = "Tableau de Bord Admin";

            // 1. Récupérer l'utilisateur actuellement connecté
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                // Cela ne devrait pas arriver si l'attribut [Authorize] fonctionne,
                // mais c'est une bonne pratique de sécurité.
                return RedirectToAction("Login", "Home");
            }

            // 2. Récupérer toutes vos données statistiques
            var classCount = _context.Classs.Count();
            var studentCount = _context.Users
                .Where(u => (u.Status == "Stagiaire") && !u.IsDeleted)
                .Count();
            var numberMen = _context.Users
                .Where(u => (u.Status == "Stagiaire") && u.Sexe == "Male")
                .Count();
            var numberWomen = _context.Users
                .Where(u => (u.Status == "Stagiaire") && u.Sexe == "Female")
                .Count();

            // 3. Préparer la liste de vos CardModels
            var cards = new List<CardModel>
            {
                new CardModel { Url = "../Classes/Class", Number = classCount.ToString(), Title = "Classes", IconUrl = "https://img.icons8.com/glyph-neue/64/classroom.png", AltText = "classroom" },
                new CardModel { Url = "../Candidatures/Candidature", Number = "254", Title = "Dossiers", IconUrl = "https://img.icons8.com/glyph-neue/64/user-folder.png", AltText = "user-folder" },
                new CardModel { Url = "../Students/Student", Number = studentCount.ToString(), Title = "Stagiaires", IconUrl = "https://img.icons8.com/glyph-neue/64/student-male.png", AltText = "student-male" },
                new CardModel { Url = "#", Number = "5", Title = "Notifications", IconUrl = "https://img.icons8.com/ios-filled/50/appointment-reminders--v1.png", AltText = "reminder" }
            };

            // 4. Créer et peupler votre DashboardViewModel
            var viewModel = new DashboardViewModel
            {
                LoggedInUser = currentUser, // L'utilisateur connecté
                Cards = cards,              // La liste des cartes
                TotalClasses = classCount,  // Les stats
                TotalStudents = studentCount,
                NumberOfMen = numberMen,
                NumberOfWomen = numberWomen
            };

            // 5. Passer le ViewModel complet à la vue
            return View(viewModel);
        }


        // DASHBOARD STAGIAIRE (STUDENT)
        [Authorize(Roles = "Student")] // Autorise SEULEMENT les utilisateurs avec le rôle "Student"
        public async Task<IActionResult> DashboardStudent()
        {
            ViewData["Title"] = "Tableau de Bord Student";

            // Récupérer l'utilisateur connecté
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Home");
            }

            // Crée un ViewModel (même si c'est un DashboardViewModel vide pour l'instant)
            // Si le tableau de bord Stagiaire a des données très différentes,
            // vous pourriez créer un 'StudentDashboardViewModel' dédié.
            var viewModel = new DashboardViewModel
            {
                LoggedInUser = currentUser,
                Cards = new List<CardModel>() // Initialisez la liste des cartes pour les stagiaires si besoin
            };

            // TODO : Ajoutez ici la logique spécifique au tableau de bord du stagiaire
            // Par exemple, récupérer les cours du stagiaire, ses notes, etc.
            // viewModel.Courses = await _context.Courses.Where(c => c.StudentId == currentUser.Id).ToListAsync();

            return View(viewModel); // Passe le ViewModel à la vue
        }
    }
}