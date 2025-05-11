using AdminMnsV1.Data;
using AdminMnsV1.Models;
using Microsoft.AspNetCore.Mvc;

namespace AdminMnsV1.Controllers
{
    public class DashboardController : Controller
    {

        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }


        // Récupère le nombre total d'étudiants
        public IActionResult Dashboard()
        {
            var studentCount = _context.Users
                .Where(u => (u.Status == "Stagiaire") && !u.IsDeleted)
                .Count();
            var cards = new List<CardModel>
            {
                new CardModel { Url = "../Classes/Class", Number = "15", Title = "Classes", IconUrl = "https://img.icons8.com/glyph-neue/64/classroom.png", AltText = "classroom" },
                new CardModel { Url = "../Candidatures/Candidature", Number = "254", Title = "Dossiers", IconUrl = "https://img.icons8.com/glyph-neue/64/user-folder.png", AltText = "user-folder" },
                new CardModel { Url = "../Students/Student", Number = studentCount.ToString(), Title = "Stagiaires", IconUrl = "https://img.icons8.com/glyph-neue/64/student-male.png", AltText = "student-male" },
                new CardModel { Url = "#", Number = "5", Title = "Notifications", IconUrl = "https://img.icons8.com/ios-filled/50/appointment-reminders--v1.png", AltText = "reminder" }
            };


            return View(cards);
        }
    }
}
