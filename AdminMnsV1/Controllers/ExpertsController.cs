using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AdminMnsV1.Models;
using AdminMnsV1.Models.Experts;
using System.Linq;
using System.Threading.Tasks;
using AdminMnsV1.Data;
using AdminMnsV1.Models.Students;

namespace AdminMnsV1.Controllers
{
    public class ExpertsController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public ExpertsController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        //*************RECUPERE LA LISTE DES EXPERTS **************
        //*************RECUPERE LA LISTE DES EXPERTS (qui sont les professeurs dans ce contexte) **************
        public async Task<IActionResult> Expert()
        {
            var professeurs = await _userManager.GetUsersInRoleAsync("Expert"); // Récupère les utilisateurs avec le rôle "Expert"
            var expertsViewModel = professeurs.Where(p => !p.IsDeleted)
                .Select(p => new ExpertEditViewModel // Utilise ton ViewModel pour l'affichage
                {
                    UserId = p.Id,
                    LastName = p.LastName,
                    FirstName = p.FirstName,
                    Sexe = p.Sexe,
                    BirthDate = p.BirthDate,
                    Address = p.Address,
                    City = p.City,
                    Email = p.Email,
                    Phone = p.PhoneNumber,
                    CreationDate = p.CreationDate,
                    Speciality = p.Speciality
                })
                .ToList();

            return View(expertsViewModel);
        }

        //*************CREATION DUN NOUVEAU INTERVENANT**********
        [HttpPost]
        public async Task<IActionResult> Create(ExpertCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var newExpert = new User // Utilise User car Expert hérite de User pour Identity
                {
                    LastName = model.LastName,
                    FirstName = model.FirstName,
                    Sexe = model.Sexe,
                    BirthDate = model.BirthDate,
                    Address = model.Address,
                    City = model.City,
                    Email = model.Email,
                    UserName = model.Email, // Important pour Identity
                    PhoneNumber = model.Phone,
                    CreationDate = DateTime.Now,
                    Status = "Expert", // Définit le statut comme Expert
                    Speciality = model.Speciality // Propriété spécifique à Expert
                };

                var result = await _userManager.CreateAsync(newExpert, model.Password);
                if (result.Succeeded)
                {
                    // Assigne le rôle Identity "Expert" à l'utilisateur
                    await _userManager.AddToRoleAsync(newExpert, "Expert");

                    TempData["SuccesMessage"] = "Le nouvel intervenant a été créé avec succès.";
                    return RedirectToAction("Expert");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View("~/Views/Experts/FormulaireExpert.cshtml", model);
                }
            }
            else
            {
                return View("~/Views/Experts/FormulaireExpert.cshtml", model);
            }
        }

        //*************MODIFIE UN INTERVENANT**********
        [HttpPost]
        public async Task<IActionResult> Modify(ExpertEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.UserId);
                if (user != null)
                {
                    user.LastName = model.LastName;
                    user.FirstName = model.FirstName;
                    user.Sexe = model.Sexe;
                    user.BirthDate = model.BirthDate;
                    user.Address = model.Address;
                    user.City = model.City;
                    user.Email = model.Email;
                    user.PhoneNumber = model.Phone;
                    user.CreationDate = model.CreationDate;
                    user.Speciality = model.Speciality; // Propriété spécifique à Expert
                    user.Status = "Expert"; // Assure que le statut reste Expert

                    var updateResult = await _userManager.UpdateAsync(user);

                    if (updateResult.Succeeded)
                    {
                        // Gérer la modification du rôle Identity si nécessaire
                        var existingRoles = await _userManager.GetRolesAsync(user);
                        if (!existingRoles.Contains("Expert"))
                        {
                            await _userManager.RemoveFromRolesAsync(user, existingRoles);
                            await _userManager.AddToRoleAsync(user, "Expert");
                        }

                        await _context.SaveChangesAsync(); // Sauvegarde les autres propriétés via le contexte EF

                        TempData["SuccesMessage"] = "Les informations de l'intervenant ont été mises à jour avec succès.";
                        return RedirectToAction("Expert");
                    }
                    else
                    {
                        foreach (var error in updateResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        return View("~/Views/Experts/ModifierExpert.cshtml", model); // Assure-toi du chemin de la vue
                    }
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                TempData["ErreurMessage"] = "Une erreur est survenue. Les informations de l'intervenant n'ont pas été mises à jour.";
                return RedirectToAction("Expert");
            }
        }
    }
}