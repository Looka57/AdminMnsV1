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
using Microsoft.AspNetCore.Mvc.Rendering;
using AdminMnsV1.Models.ViewModels;

namespace AdminMnsV1.Controllers
{
    public class StudentsController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<StudentsController> _logger;
        private readonly IWebHostEnvironment _environment; //Injecte le service qui fournit des informations sur l'environnement web de l'application, y compris le chemin racine du contenu web (wwwroot).



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
            var students = _context.Set<Student>() // Cible le DbSet de Student
            .Where(s => (s.Status == "Stagiaire" || s.Status == "Candidat") && !s.IsDeleted)
            .Include(s => s.Attends) // <-- MAINTENANT s est de type Student, donc s.Attends est valide
            .ThenInclude(a => a.Class) // Inclure la Classe pour chaque Attend (Class car c'est le nom de la prop dans Attend.cs)
         .ToList();

            var studentViewModels = students.Select(s => new StudentEditViewModel
            {
                UserId = s.Id,
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
                Role = s.Status, // Ici, tu utilises le Status de User comme Role dans le ViewModel
                SocialSecurityNumber = s.SocialSecurityNumber,
                FranceTravailNumber = s.FranceTravailNumber,
                Photo = s.Photo,
                //Classe = s.Attends.FirstOrDefault()?.Class?.NameClass, // 's' est valide ici
                ClassesAttended = s.Attends.Select(a => a.Class?.NameClass).ToList(), // 's' est valide ici

                ClassId = s.Attends.Select(a => a.ClasseId).FirstOrDefault() // .FirstOrDefault() prend le premier ID trouvé
                                                                               // Si vous avez une date dans Attend (ex: a.EnrollmentDate), vous pourriez prendre la plus récente :
                                                                               // ClassId = s.Attends.OrderByDescending(a => a.EnrollmentDate).Select(a => a.ClasseId).FirstOrDefault()


            }).ToList();



            // Récupérez TOUTES les classes disponibles pour le menu déroulant (une seule fois)
            var classesFromDb = _context.Classs.OrderBy(c => c.NameClass).ToList(); 
            var availableClassesSelectList = new SelectList(classesFromDb, "ClasseId", "NameClass");

            //Créez le ViewModel de la page et peuplez-le
            var pageViewModel = new StudentListPageViewModel
            {
                Students = studentViewModels, // Assignez la liste des étudiants
                AvailableClasses = availableClassesSelectList // Assignez la SelectList globale
            };

            return View(pageViewModel);
        }


        //*************CRRER UN STAGIAIRES **********


        // Dans StudentsController.cs (action GET Create)
        public IActionResult Create()
        {
            var viewModel = new StudentCreateViewModel();

            // --- C'est ici que le problème prend racine s'il y a un Null ---
            var classesFromDb = _context.Classs.OrderBy(c => c.NameClass).ToList(); // Ligne 1 : Récupère les données
            viewModel.AvailableClasses = new SelectList(classesFromDb, "ClasseId", "NameClass"); // Ligne 2 : Crée et assigne le SelectList
                                                                                                 // --- Fin ---

            return View("~/Views/Students/Formulaire.cshtml", viewModel); // Ligne 3 : Envoie le ViewModel (qui contient AvailableClasses) à la vue
        }


        // Dans StudentsController.cs

    

        //*************GÈRE LA SOUMISSION DU FORMULAIRE DE CRÉATION (POST) **********
        [HttpPost]
        public async Task<IActionResult> Create(StudentCreateViewModel model)
        {
            // --- Point de départ : Vérifie si les données soumises sont valides selon les règles du ViewModel ---
            // Les attributs [Required], [MaxLength], etc. sur les propriétés du ViewModel sont vérifiés ici.
            if (ModelState.IsValid)
            {
                string? uniqueFileName = "default_profile.png"; // Nom de votre image par défaut

                if (model.PhotoFile != null && model.PhotoFile.Length > 0)
                {
                    string uploadFolder = Path.Combine(_environment.WebRootPath, "images", "Profiles");
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + model.PhotoFile.FileName;
                    string filePath = Path.Combine(uploadFolder, uniqueFileName);
                    Directory.CreateDirectory(uploadFolder);
                    using var fileStream = new FileStream(filePath, FileMode.Create);
                    await model.PhotoFile.CopyToAsync(fileStream);
                }

                // 2. Crée l'objet newUser (Student) (Votre code existant)
                var newUser = new Student // Utilise Student si Student hérite de User pour Identity
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
                    Status = model.Status, // Utilise le Status du ViewModel
                    SocialSecurityNumber = model.SocialSecurityNumber,
                    FranceTravailNumber = model.FranceTravailNumber,
                    Photo = uniqueFileName,
                };

                // 3. Tente de créer l'utilisateur via UserManager (Votre code existant - Opération asynchrone)
                var result = await _userManager.CreateAsync(newUser, model.Password);

                // 4. Vérifie si la création de l'utilisateur par Identity a réussi
                if (result.Succeeded) // <-- Début du deuxième IF (imbriqué) : SI UserManager réussit
                {
                    // --- Ici va le code qui s'exécute SEULEMENT si la création Identity réussit ---

                    // 5. Crée l'entrée dans la table Attend (Votre code existant)
                    var attendEntry = new Attend
                    {
                        StudentId = newUser.Id, // L'ID du nouvel utilisateur/étudiant
                        ClasseId = model.ClassId, // L'ID de la classe sélectionnée
                                                  // EnrollmentDate = DateTime.UtcNow // Date d'inscription (si non commenté)
                    };
                    _context.Attends.Add(attendEntry); // Ajoutez au contexte
                    await _context.SaveChangesAsync(); // Sauvegardez dans la BDD

                    // 6. Redirige en cas de succès complet (Votre code existant)
                    Console.WriteLine($"Utilisateur créé avec succès, Statut = {newUser.Status}");
                    TempData["SuccesMessage"] = "Le nouveau stagiaire a été créé et inscrit à la classe sélectionnée avec succès.";
                    return RedirectToAction("Student"); // Redirige
                }
                else // <-- Début du deuxième ELSE (imbriqué) : SINON (Si UserManager échoue)
                {
                    // --- Ici va le code qui s'exécute si la création Identity échoue (ex: email déjà utilisé) ---

                    // 7. Ajoute les erreurs de UserManager à ModelState (Votre code existant)
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    Console.WriteLine($"Erreur lors de la création de l'utilisateur.");

                    // 8. Re-popule model.AvailableClasses (Nécessaire pour que le menu déroulant s'affiche si on retourne la vue)
                    var classesFromDb = _context.Classs.OrderBy(c => c.NameClass).ToList(); 
                    model.AvailableClasses = new SelectList(classesFromDb, "ClasseId", "NameClass");

                    // 9. Retourne la vue avec le modèle (contenant les erreurs de UserManager et la liste des classes)
                    return View("~/Views/Students/Formulaire.cshtml", model);
                } // <-- Fin du deuxième ELSE (imbriqué)
            } // <-- Fin du premier grand IF (celui de ModelState.IsValid)

            // --- Début du premier grand ELSE : SINON (Si la validation initiale du modèle ÉCHOUE) ---
            else // <-- CE BLOC ELSE DOIT IMMEDIATEMENT SUIVRE LE PREMIER GRAND IF
            {
                // --- Ici va le code qui s'exécute lorsque la validation initiale échoue (ex: champ Nom vide si [Required]) ---
                // Les erreurs de validation sont déjà dans ModelState grâce aux attributs ([Required], [MaxLength], etc.)

                // 10. Re-popule model.AvailableClasses (Nécessaire pour que le menu déroulant s'affiche si on retourne la vue)
                var classesFromDb = _context.Classs.OrderBy(c => c.NameClass).ToList(); // <-- CORRIGEZ "Classs" ici !
                model.AvailableClasses = new SelectList(classesFromDb, "ClasseId", "NameClass");

                // 11. Retourne la vue avec le modèle (contenant les erreurs de validation et la liste re-populée)
                //ModelState.Remove(nameof(model.AvailableClasses));
                return View("~/Views/Students/Formulaire.cshtml", model);
            } // <-- Fin du premier grand ELSE

            // Le code de l'action se termine ici. Il n'y a pas de code ou d'autres blocs 'else' après ce grand if/else.
        } // <-- Fin de l'action Create [HttpPost]



        //*************MODIFIE UN STAGIAIRE**********
        // Dans StudentsController.cs

        [HttpPost]
        public async Task<IActionResult> Modify(StudentEditViewModel model)
        {
            // 1. Vérifier si les données du ViewModel sont valides (selon les attributs [Required], etc.)
            if (!ModelState.IsValid)
            {
                // Rediriger avec un message d'erreur si la validation échoue
                TempData["ErreurMessage"] = "Informations invalides fournies.";
                return RedirectToAction("Student"); // Redirige vers la page de liste en cas d'échec de validation
            }

            // 2. Trouver le stagiaire existant dans la base de données
            var user = await _userManager.FindByIdAsync(model.UserId);

            // Vérifier si l'utilisateur a été trouvé
            if (user == null)
            {
                TempData["ErreurMessage"] = "Stagiaire non trouvé.";
                return RedirectToAction("Student"); // Redirige si l'utilisateur n'existe pas
            }

            // 3. Mettre à jour les propriétés de base de l'entité User (ou Student) avec les valeurs du ViewModel
            var originalCreationDate = user.CreationDate; // Conserve l'original
            user.LastName = model.LastName;
            user.FirstName = model.FirstName;
            user.Sexe = model.Sexe;
            user.BirthDate = model.BirthDate;
            user.Nationality = model.Nationality;
            user.Address = model.Address;
            user.City = model.City;
            user.Email = model.Email;
            user.Phone = model.Phone;
            // user.CreationDate = model.CreationDate; // NE PAS FAIRE CECI
            user.Status = model.Role; // Met à jour le statut de User
            user.SocialSecurityNumber = model.SocialSecurityNumber; // N'oubliez pas ces champs
            user.FranceTravailNumber = model.FranceTravailNumber;

            // Réassigne la date de création originale (si nécessaire, sinon pas besoin)
            user.CreationDate = originalCreationDate;


            // 4. *** SAUVEGARDER LES MODIFICATIONS DE L'ENTITÉ USER via UserManager EN PREMIER ***
            var updateResult = await _userManager.UpdateAsync(user);

            // Vérifier si la sauvegarde User a réussi
            if (!updateResult.Succeeded)
            {
                string errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
                TempData["ErreurMessage"] = $"Erreur Identity lors de la mise à jour du stagiaire : {errors}";
                return RedirectToAction("Student"); // Redirige en cas d'échec de sauvegarde User Identity
            }


            // 5. *** LOGIQUE POUR METTRE À JOUR LA CLASSE DANS LA TABLE Attends ***
            // (Utilise model.ClassId reçu du formulaire de la modale)

            // Récupérer les entrées existantes dans la table Attends pour ce stagiaire (en utilisant user.Id)
            var existingAttends = _context.Attends.Where(a => a.StudentId == user.Id).ToList();

            // Option simple : Supprimer toutes les liaisons existantes
            if (existingAttends.Any())
            {
                _context.Attends.RemoveRange(existingAttends); // Marque les anciennes entrées pour suppression
            }

            // Si une classe a été sélectionnée (model.ClassId > 0), ajouter la nouvelle liaison
            if (model.ClassId > 0)
            {
                var newAttend = new Attend // Assurez-vous d'avoir une classe modèle Attend
                {
                    StudentId = user.Id, // L'ID de l'utilisateur/stagiaire (string)
                    ClasseId = model.ClassId, // L'ID de la classe sélectionnée (int)
                                              // Ajoutez EnrollmentDate si applicable
                                              // EnrollmentDate = DateTime.UtcNow
                };
                _context.Attends.Add(newAttend); // Ajoute la nouvelle entrée au contexte
            }
            // Si model.ClassId est 0, aucune nouvelle liaison n'est ajoutée après la suppression des anciennes.


            // 6. *** SAUVEGARDER LES CHANGEMENTS DANS LE CONTEXTE DE LA BASE DE DONNÉES pour les entités Attend ***
            // Appelez SaveChangesAsync() UNE SEULE FOIS pour sauvegarder toutes les modifications sur les entités non-Identity.
            // C'est ici que les suppressions et additions sur _context.Attends sont écrites en BDD.
            await _context.SaveChangesAsync();


            // 7. Afficher un message de succès et rediriger vers la page de liste
            TempData["SuccesMessage"] = $"Le stagiaire {user.FirstName} {user.LastName} a été mis à jour avec succès.";
            return RedirectToAction("Student"); // Redirige vers l'action qui affiche la liste
        }





        ////*************SUPPRIME UN STAGIAIRE**********

        [HttpPost]
        public async Task<IActionResult> Delete(string id) // L'ID d'un utilisateur dans AspNetUsers est un string
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                TempData["ErreurMessage"] = "Stagiaire non trouvé."; // Ajoutez cette ligne
                return RedirectToAction("Student"); // Changez ici pour rediriger au lieu de NotFound
                                                    // Ou si c'était déjà RedirectToAction, assurez-vous du message TempData
            }

            user.IsDeleted = true;
            var updateResult = await _userManager.UpdateAsync(user); // Utilise UserManager pour mettre à jour l'utilisateur

            if (!updateResult.Succeeded)
            {
                string errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
                TempData["ErreurMessage"] = $"Erreur lors du marquage du stagiaire comme supprimé : {errors}";
                return RedirectToAction(nameof(Student)); // Retourne en cas d'échec de la mise à jour
            }
        

            TempData["SuccesMessage"] = $"L'utilisateur {user.FirstName} {user.LastName} a été marqué comme supprimé.";
            return RedirectToAction(nameof(Student));
        }
    }
}