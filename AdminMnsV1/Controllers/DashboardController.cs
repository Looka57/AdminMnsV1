using AdminMnsV1.Data;
using AdminMnsV1.Models;
using Microsoft.AspNetCore.Authorization;


using Microsoft.AspNetCore.Mvc;

namespace AdminMnsV1.Controllers
{
        [Authorize]
    public class DashboardController : Controller
    {

        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        // DASHBOARD ADMIN
        [Authorize(Roles = "Admin")]
        public IActionResult Dashboard()
        {
            ViewData["Title"] = "Tableau de Bord Admin";

            //recuper le nombre total de classes 
            var classCount = _context.Classs
           .Count();

            // Récupère le nombre total d'étudiants
            var studentCount = _context.Users
                .Where(u => (u.Status == "Stagiaire") && !u.IsDeleted)
                .Count();


            //Recupere le nombre d'hommes'
            var numberMen = _context.Users
                .Where(u => (u.Status == "Stagiaire") && u.Sexe == "Male")
                .Count();

            //Recupere le nombre de femmes
            var numberWomen = _context.Users
                .Where(u => (u.Status == "Stagiaire") && u.Sexe == "Female")
                .Count();



            var cards = new List<CardModel>
            {
                new CardModel { Url = "../Classes/Class", Number = classCount.ToString(), Title = "Classes", IconUrl = "https://img.icons8.com/glyph-neue/64/classroom.png", AltText = "classroom" },
                new CardModel { Url = "../Candidatures/Candidature", Number = "254", Title = "Dossiers", IconUrl = "https://img.icons8.com/glyph-neue/64/user-folder.png", AltText = "user-folder" },
                new CardModel { Url = "../Students/Student", Number = studentCount.ToString(), Title = "Stagiaires", IconUrl = "https://img.icons8.com/glyph-neue/64/student-male.png", AltText = "student-male" },
                new CardModel { Url = "#", Number = "5", Title = "Notifications", IconUrl = "https://img.icons8.com/ios-filled/50/appointment-reminders--v1.png", AltText = "reminder" }
            };

            ViewBag.NombreHommes = numberMen;
            ViewBag.NombreFemmes = numberWomen; // Passe la valeur pour les femmes graphique



            return View(cards);
        }

        // DASHBOARD STAGIAIRE
        // CETTE ACTION MANQUAIT DANS VOTRE CODE PRÉCÉDENT
        // Applique l'autorisation : Seuls les utilisateurs avec le rôle "Stagiaire" peuvent accéder à cette action.
        //[Authorize(Roles = "Student")]

        // DASHBOARD STUDENT (STAGIAIRE)
        [Authorize(Roles = "Student")] // <<< Autorise SEULEMENT les utilisateurs avec le rôle "Student"
        public IActionResult DashboardStudent()
        {
            ViewData["Title"] = "Tableau de Bord Student"; // Vous pouvez changer en "Student Dashboard"
            // TODO : Ajoutez ici la logique spécifique au tableau de bord du stagiaire
            return View(); // Cela va chercher Views/Dashboard/DashboardStudent.cshtml
        }
    }
}
