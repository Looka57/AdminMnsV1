using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AdminMnsV1.Models;
using AdminMnsV1.Models.Students;
using System.Threading.Tasks;
using AdminMnsV1.Data;
using System.Linq;
using AdminMnsV1.Models;

namespace AdminMnsV1.Controllers
{
    public class StudentsController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public StudentsController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        //*************RECUPERE LES STAGIAIRES **********
        public IActionResult Student()
        {
            var users = _context.Users
                .Where(u => (u.Status == "Stagiaire" || u.Status == "Candidat") && !u.IsDeleted)
                .ToList();

            var studentViewModels = users.Select(u => new StudentEditViewModel
            {
                UserId = u.Id,
                LastName = u.LastName,
                FirstName = u.FirstName,
                Sexe = u.Sexe,
                BirthDate = u.BirthDate,
                Nationality = u.Nationality,
                Address = u.Address,
                City = u.City,
                Email = u.Email,
                Phone = u.Phone,
                CreationDate = u.CreationDate,
                Role = u.Status, // Ici, tu utilises le Status de User comme Role dans le ViewModel
                SocialSecurityNumber = u.SocialSecurityNumber,
                FranceTravailNumber = u.FranceTravailNumber
            }).ToList();

            return View(studentViewModels);
        }


        [HttpPost]
        public async Task<IActionResult> Create(StudentCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var newUser = new User // Utilise User car Student hérite de User pour Identity
                {
                    LastName = model.LastName,
                    FirstName = model.FirstName,
                    Sexe = model.Sexe,
                    BirthDate = model.BirthDate,
                    Nationality = model.Nationality,
                    Address = model.Address,
                    City = model.City,
                    Phone = model.Phone,
                    CreationDate = DateTime.Now,
                    Email = model.Email,
                    UserName = model.Email, // Important pour Identity
                    Status = model.Status,
                    SocialSecurityNumber = model.SocialSecurityNumber,
                    FranceTravailNumber = model.FranceTravailNumber
                };

                var result = await _userManager.CreateAsync(newUser, model.Password);
                if (result.Succeeded)
                {
                    // Assigne le rôle Identity "Student" à l'utilisateur
                    await _userManager.AddToRoleAsync(newUser, "Student");

                    TempData["SuccesMessage"] = "Le nouveau stagiaire a été créé avec succès.";
                    return RedirectToAction("Student");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View("~/Views/Students/Formulaire.cshtml", model);
                }
            }
            else
            {
                return View("~/Views/Students/Formulaire.cshtml", model);
            }
        }

        //*************MODIFIE UN STAGIAIRE**********
        [HttpPost]
        public async Task<IActionResult> Modify(StudentEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.UserId);
                if (user != null)
                {
                    // Conserve la date de création originale
                    var originalCreationDate = user.CreationDate;

                    user.LastName = model.LastName;
                    user.FirstName = model.FirstName;
                    user.Sexe = model.Sexe;
                    user.BirthDate = model.BirthDate;
                    user.Nationality = model.Nationality;
                    user.Address = model.Address;
                    user.City = model.City;
                    user.Email = model.Email;
                    user.Phone = model.Phone; 
                    user.CreationDate = model.CreationDate;
                    user.Status = model.Role; // Assigne la valeur du rôle du ViewModel au statut

                    // Réassigne la date de création originale
                    user.CreationDate = originalCreationDate;

                    var updateResult = await _userManager.UpdateAsync(user);

                    if (updateResult.Succeeded)
                    {
                        // Gérer la modification du rôle Identity si nécessaire
                        // Récupérer les rôles actuels de l'utilisateur
                        var existingRoles = await _userManager.GetRolesAsync(user);
                        if (!existingRoles.Contains("Student"))
                        {
                            // Supprimer les rôles existants (si tu veux un seul rôle)
                            await _userManager.RemoveFromRolesAsync(user, existingRoles);
                            // Ajouter l'utilisateur au rôle "Student"
                            await _userManager.AddToRoleAsync(user, "Student");
                        }
                        else if (model.Role != "Stagiaire" && model.Role != "Candidat")
                        {
                            // Si le rôle dans le ViewModel a changé et n'est plus "Stagiaire" ou "Candidat"
                            // Tu peux ajouter une logique ici pour gérer d'autres rôles Identity si nécessaire
                        }

                        _context.SaveChanges(); // Sauvegarde les autres propriétés via le contexte EF

                        TempData["SuccesMessage"] = "Les informations de l'étudiant ont été mises à jour avec succès.";
                        return RedirectToAction("Student");
                    }
                    else
                    {
                        foreach (var error in updateResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        return View("~/Views/Students/Modifier.cshtml", model); // Assure-toi que le chemin est correct
                    }
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                TempData["ErreurMessage"] = "Une erreur est survenue. Les informations de l'étudiant n'ont pas été mises à jour.";
                return RedirectToAction("Student");
            }

        }
        ////*************SUPPRIME UN STAGIAIRE**********

        [HttpPost]
        public async Task<IActionResult> Delete(string id) // L'ID d'un utilisateur dans AspNetUsers est un string
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            user.IsDeleted = true;
            await _userManager.UpdateAsync(user); // Utilise UserManager pour mettre à jour l'utilisateur

            TempData["SuccesMessage"] = $"L'utilisateur {user.FirstName} {user.LastName} a été marqué comme supprimé.";
            return RedirectToAction(nameof(Student));
        }
    }
}