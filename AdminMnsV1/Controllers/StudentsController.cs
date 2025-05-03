using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using AdminMnsV1.Models;
using AdminMnsV1.Data;

namespace AdminMnsV1.Controllers
{
    public class StudentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Student()
        {
            // Récupère tous les utilisateurs qui sont de type Student
            var students = _context.Users
                .OfType<Student>() 
                .ToList();

            // Passe la liste des stagiaires à la vue nommée "Student"
            return View(students);
        }
    }
}