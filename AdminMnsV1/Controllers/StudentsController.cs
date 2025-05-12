using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AdminMnsV1.Models;
using AdminMnsV1.Models.Students;
using System.Threading.Tasks;
using AdminMnsV1.Data;
using System.Linq;
using AdminMnsV1.Models;
using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.IdentityModel.Tokens;
using System.IO;
using System.Runtime.ConstrainedExecution;
using Microsoft.EntityFrameworkCore;

namespace AdminMnsV1.Controllers
{
    public class StudentsController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment; //Injecte le service qui fournit des informations sur l'environnement web de     l'application, y compris le chemin racine du contenu web (wwwroot).


        public StudentsController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _environment = environment;
        }

        //*************RECUPERE LES STAGIAIRES **********

        public IActionResult Student()
        {
            //tableau de tout les students
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
                FranceTravailNumber = u.FranceTravailNumber,
                Photo = u.Photo
            }).ToList();

            return View(studentViewModels);
        }


        //*************CRRER UN STAGIAIRES **********

        [HttpPost]
        public async Task<IActionResult> Create(StudentCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                string? uniqueFileName = null;
                //Vérification du fichier
                if (model.PhotoFile != null && model.PhotoFile.Length > 0) //Vérifie si model.PhotoProfil n'est pas null et si sa longueur est supérieure à zéro
                {
                    string uploadFolder = Path.Combine(_environment.WebRootPath, "images", "Profiles"); // Crée un dossier pour les photos
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + model.PhotoFile.FileName; //Création du nom de fichier unique 
                    string filePath = Path.Combine(uploadFolder, uniqueFileName);
                    //Combine le chemin racine web(wwwroot), un dossier "images/profiles" et le nom de fichier unique pour obtenir le chemin complet où le fichier sera enregistré sur le serveur.

                    Directory.CreateDirectory(uploadFolder);   // Verification que le dossier existe

                    using var fileStream = new FileStream(filePath, FileMode.Create); //Sauvegarde du fichier: 
                                                                                      //FileStream pour créer un fichier au chemin spécifié  
                    await model.PhotoFile.CopyToAsync(fileStream); //copier le contenu du fichier téléchargé vers ce fichier sur le serveur*/.
                }

                var newUser = new Student // Utilise User car Student hérite de User pour Identity (discriminator)
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
                    Status = model.Status, // Stagiaire ou Candidat
                    SocialSecurityNumber = model.SocialSecurityNumber,
                    FranceTravailNumber = model.FranceTravailNumber,
                    Photo = uniqueFileName // Sauvegarde le nom du fichier (ou null si aucun fichier n'a été téléchargé)
                    //Le nom de fichier unique (uniqueFileName) est stocké dans la propriété Photo de l'objet newUser
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
                        return View("~/Views/Students/Modifier.cshtml", model);
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