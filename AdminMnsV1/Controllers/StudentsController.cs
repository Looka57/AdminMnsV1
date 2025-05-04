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

            // Crée une liste de StudentEditViewModel à partir de la liste des Students
            var studentViewModels = students.Select(s => new StudentEditViewModel
            {
                UserId = s.UserId,
                LastName = s.LastName,
                FirstName = s.FirstName,
                Sexe = s.Sexe,
                BirthDate = s.BirthDate,
                Nationality = s.Nationality,
                Address = s.Address,
                City = s.City,
                Email = s.Email,
                Phone = s.Phone,
                CreationDate = s.CreationDate,
                Role = s.Role,
                SocialSecurityNumber = s.SocialSecurityNumber,
                FranceTravailNumber = s.FranceTravailNumber
                // N'incluez pas PasswordHash ou Discriminator ici
            }).ToList();

            // Passe la liste des StudentEditViewModel à la vue nommée "Student"
            return View(studentViewModels);
        }
        


        [HttpPost]
        public IActionResult Modify(StudentEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var student = _context.Students.Find(model.UserId);
                if (student != null)
                {
                    student.LastName = model.LastName;
                    student.FirstName = model.FirstName;
                    student.Sexe = model.Sexe;
                    student.BirthDate = model.BirthDate;
                    student.Nationality = model.Nationality;
                    student.Address = model.Address;
                    student.City = model.City;
                    student.Email = model.Email;
                    student.Phone = model.Phone;
                    student.CreationDate = model.CreationDate;
                    student.Role = model.Role;
                    student.SocialSecurityNumber = model.SocialSecurityNumber;
                    student.FranceTravailNumber = model.FranceTravailNumber;
                }

                // Enregistrer les changements dans la base de données
                _context.SaveChanges();
                // Rediriger l'utilisateur vers la liste des stagiaires
                return RedirectToAction("Student");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("--- Etat complet de ModelState (après suppression de Discriminator) ---");
                foreach (var keyValuePair in ModelState)
                {
                    System.Diagnostics.Debug.WriteLine($"Clé: {keyValuePair.Key}, Etat: {keyValuePair.Value.ValidationState}, Erreurs: {string.Join(", ", keyValuePair.Value.Errors.Select(e => e.ErrorMessage))}");
                }
                System.Diagnostics.Debug.WriteLine("------------------------------------------------------------------");
                return BadRequest(ModelState);
            }
        }
        }
    }
