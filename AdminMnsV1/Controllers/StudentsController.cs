using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using AdminMnsV1.Data;
using AdminMnsV1.Models.Students;

namespace AdminMnsV1.Controllers
{
    public class StudentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudentsController(ApplicationDbContext context)
        {
            _context = context;
        }


        //*************RECUPERE LES STAGIAIRES **********
        public IActionResult Student()
        {
            // Récupère tous les utilisateurs qui sont de type Student
            var students = _context.Users
                .OfType<Student>()
                .Where(s=> !s.IsDeleted)
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

            // Passe la liste des StudentEditViewModel à la vue nommée "Student"z
            return View(studentViewModels);
        }


        //*************CREATION DUN NOUVEAU STAGIAIRE**********
        [HttpPost]
        public IActionResult Create(StudentCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var newStudent = new Student
                {
                    LastName = model.LastName,
                    FirstName = model.FirstName,
                    Sexe = model.Sexe,
                    BirthDate = model.BirthDate,
                    Nationality = model.Nationality,
                    Address = model.Address,
                    City = model.City,
                    Email = model.Email,
                    PasswordHash = model.Password,// Assignation directe du mot de passe (TEMPORAIRE)
                    Phone = model.Phone,
                    CreationDate = DateTime.Now,
                    Role = model.Role,
                    SocialSecurityNumber = model.SocialSecurityNumber,
                    FranceTravailNumber = model.FranceTravailNumber
                };

                _context.Users.Add(newStudent);
                _context.SaveChanges();

                TempData["SuccesMessage"] = "Le nouveau stagiaire a été créé avec succès.";
                return RedirectToAction("Student");
            }
            else
            {
                return View("~/Views/Students/Formulaire.cshtml", model);
            }
        }

        //*************MODIFIE UN STAGIAIRE**********
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

                    // Enregistrer les changements dans la base de données
                    _context.SaveChanges();

                    //Ajouter un message de succes a la bibliotheque TempData
                    TempData["SuccesMessage"] = "Les informations de l'étudiant ont été mises à jour avec succès.";

                    // Rediriger l'utilisateur vers la liste des stagiaires
                    return RedirectToAction("Student");
                }
                else
                {
                    // ... gestion si l'étudiant n'est pas trouvé ...
                    return NotFound();
                }
            }

            else
            {

                //Ajouter un message de succes a la bibliotheque TempData
                TempData["ErreurMessage"] = "Une erreur est survenue. Les informations de l'étudiant n'ont pas été mises à jour.";

                return RedirectToAction("Student"); // Ou votre logique de gestion des erreurs
            }
        }

        //*************SUPPRIME UN STAGIAIRE**********

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var student = await _context.Students.FindAsync(id);

            if (student == null)
            {
                return NotFound();
            }

            student.IsDeleted = true;
            _context.Update(student);
            await _context.SaveChangesAsync();

            TempData["SuccesMessage"] = $"Le stagiaire {student.FirstName} {student.LastName} a été supprimé."; // Message de succès
            return RedirectToAction(nameof(Student)); // Rediriger vers l'action qui liste les stagiaires
        }
    }
}
