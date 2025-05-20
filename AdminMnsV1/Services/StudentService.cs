// Services/StudentService.cs
using AdminMnsV1.Models;
using AdminMnsV1.Models.Students;
using AdminMnsV1.Models.ViewModels;
using AdminMnsV1.Repositories.Interfaces;
using AdminMnsV1.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace AdminMnsV1.Services
{
    public class StudentService : IStudentService
    {
        private readonly UserManager<User> _userManager;
        private readonly IStudentRepository _studentRepository;
        private readonly IClassRepository _classRepository;
        private readonly IWebHostEnvironment _environment;

        public StudentService(
            UserManager<User> userManager,
            IStudentRepository studentRepository,
            IClassRepository classRepository,
            IWebHostEnvironment environment)
        {
            _userManager = userManager;
            _studentRepository = studentRepository;
            _classRepository = classRepository;
            _environment = environment;
        }

        public async Task<StudentListPageViewModel> GetStudentListPageViewModelAsync()
        {
            var students = await _studentRepository.GetAllStudentsWithDetailsAsync();

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
                Role = s.Status,
                SocialSecurityNumber = s.SocialSecurityNumber,
                FranceTravailNumber = s.FranceTravailNumber,
                Photo = s.Photo,
                ClassesAttended = s.Attends?.Select(a => a.Class?.NameClass).ToList() ?? new List<string>(),
                ClassId = s.Attends?.Select(a => a.ClasseId).FirstOrDefault() ?? 0
            }).ToList();

            var classesFromDb = await _classRepository.GetAllClassesAsync();
            var availableClassesSelectList = new SelectList(classesFromDb, "ClasseId", "NameClass");

            return new StudentListPageViewModel
            {
                Students = studentViewModels,
                AvailableClasses = availableClassesSelectList
            };
        }

        public async Task<StudentCreateViewModel> GetStudentCreateViewModelAsync()
        {
            var classesFromDb = await _classRepository.GetAllClassesAsync();
            var viewModel = new StudentCreateViewModel
            {
                AvailableClasses = new SelectList(classesFromDb, "ClasseId", "NameClass")
            };
            return viewModel;
        }

        public async Task<(IdentityResult Result, string? ErrorMessage)> CreateStudentAsync(StudentCreateViewModel model)
        {
            string? uniqueFileName = "default_profile.png";
            if (model.PhotoFile != null && model.PhotoFile.Length > 0)
            {
                string uploadFolder = Path.Combine(_environment.WebRootPath, "images", "Profiles");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.PhotoFile.FileName;
                string filePath = Path.Combine(uploadFolder, uniqueFileName);
                Directory.CreateDirectory(uploadFolder); // S'assurer que le dossier existe
                using var fileStream = new FileStream(filePath, FileMode.Create);
                await model.PhotoFile.CopyToAsync(fileStream);
            }

            var newUser = new Student
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
                UserName = model.Email,
                Status = model.Status,
                SocialSecurityNumber = model.SocialSecurityNumber,
                FranceTravailNumber = model.FranceTravailNumber,
                Photo = uniqueFileName,
            };

            var createResult = await _userManager.CreateAsync(newUser, model.Password);

            if (!createResult.Succeeded)
            {
                string errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                return (createResult, $"Erreur lors de la création de l'utilisateur : {errors}");
            }

            // Ajout au rôle
            var roleResult = await _userManager.AddToRoleAsync(newUser, "Student");
            if (!roleResult.Succeeded)
            {
                string errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                // Considérer de supprimer l'utilisateur si l'ajout de rôle échoue
                await _userManager.DeleteAsync(newUser);
                return (roleResult, $"Erreur lors de l'attribution du rôle 'Student' : {errors}");
            }

            // Création de l'entrée Attend (inscription à la classe)
            if (model.ClassId > 0)
            {
                var attendEntry = new Attend
                {
                    StudentId = newUser.Id,
                    ClasseId = model.ClassId,
                    // EnrollmentDate = DateTime.UtcNow // Si tu l'as
                };
                await _studentRepository.AddAttendEntryAsync(attendEntry);
                await _studentRepository.SaveChangesAsync(); // Sauvegarde l'entrée Attend
            }

            return (createResult, null); // Succès
        }

        public async Task<(IdentityResult Result, string? ErrorMessage)> UpdateStudentAsync(StudentEditViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return (IdentityResult.Failed(new IdentityError { Description = "Stagiaire non trouvé." }), "Stagiaire non trouvé.");
            }

            // Mettre à jour les propriétés du modèle User
            user.LastName = model.LastName;
            user.FirstName = model.FirstName;
            user.Sexe = model.Sexe;
            user.BirthDate = model.BirthDate;
            user.Nationality = model.Nationality;
            user.Address = model.Address;
            user.City = model.City;
            user.Email = model.Email;
            user.PhoneNumber = model.Phone; // Utilise PhoneNumber de IdentityUser
            user.UserName = model.Email; // Mettre à jour le UserName aussi si Email est changé
            user.Status = model.Role;
            user.SocialSecurityNumber = model.SocialSecurityNumber;
            user.FranceTravailNumber = model.FranceTravailNumber;
            // La photo n'est pas gérée dans le ViewModel d'édition, tu devras ajouter un IFormFile ici si tu veux la modifier.

            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                string errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
                return (updateResult, $"Erreur Identity lors de la mise à jour du stagiaire : {errors}");
            }

            // Gérer la classe Attends
            await _studentRepository.RemoveAttendEntriesAsync(user.Id); // Supprime toutes les entrées existantes
            if (model.ClassId > 0)
            {
                var newAttend = new Attend
                {
                    StudentId = user.Id,
                    ClasseId = model.ClassId,
                };
                await _studentRepository.AddAttendEntryAsync(newAttend);
            }
            await _studentRepository.SaveChangesAsync(); // Sauvegarder les changements sur Attends

            return (updateResult, null); // Succès
        }

        public async Task<(IdentityResult Result, string? ErrorMessage)> SoftDeleteStudentAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return (IdentityResult.Failed(new IdentityError { Description = "Stagiaire non trouvé." }), "Stagiaire non trouvé.");
            }

            user.IsDeleted = true; // Marquer comme supprimé logiquement
            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                string errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
                return (updateResult, $"Erreur lors du marquage du stagiaire comme supprimé : {errors}");
            }

            return (updateResult, null); // Succès
        }

        public async Task<StudentEditViewModel?> GetStudentEditViewModelAsync(string id)
        {
            var student = await _studentRepository.GetStudentByIdAsync(id);
            if (student == null)
            {
                return null;
            }

            var classesFromDb = await _classRepository.GetAllClassesAsync();
            var availableClassesSelectList = new SelectList(classesFromDb, "ClasseId", "NameClass");

            return new StudentEditViewModel
            {
                UserId = student.Id,
                LastName = student.LastName,
                FirstName = student.FirstName,
                Sexe = student.Sexe,
                BirthDate = student.BirthDate,
                Nationality = student.Nationality,
                Address = student.Address,
                City = student.City,
                Email = student.Email,
                Phone = student.Phone,
                CreationDate = student.CreationDate,
                Role = student.Status,
                SocialSecurityNumber = student.SocialSecurityNumber,
                FranceTravailNumber = student.FranceTravailNumber,
                Photo = student.Photo,
                ClassesAttended = student.Attends?.Select(a => a.Class?.NameClass).ToList() ?? new List<string>(),
                ClassId = student.Attends?.Select(a => a.ClasseId).FirstOrDefault() ?? 0,
                // AvailableClasses ne peut pas être dans EditViewModel s'il n'est pas utilisé pour un SelectList dans la modale d'édition.
                // Si tu utilises la modale d'édition pour choisir une classe, tu devras l'ajouter.
            };
        }
    }
}

//**Explication :**

//  *Le service gère le flux complet pour chaque opération :
//      ***Récupération * *(`GetStudentListPageViewModelAsync`, `GetStudentCreateViewModelAsync`, `GetStudentEditViewModelAsync`) : Il appelle les repositories pour les données brutes, puis construit les ViewModels pour le contrôleur.
//      * **Création** (`CreateStudentAsync`) : Gère l'upload de photo, crée l'objet `Student`, appelle `_userManager.CreateAsync` et `_userManager.AddToRoleAsync`, puis utilise `_studentRepository` pour l'inscription `Attend`.
//      * **Mise à jour** (`UpdateStudentAsync`) : Utilise `_userManager.FindByIdAsync` et `_userManager.UpdateAsync` pour les propriétés `IdentityUser` et `_studentRepository` pour les relations `Attend`.
//      * **Suppression** (`SoftDeleteStudentAsync`) : Met à jour la propriété `IsDelete