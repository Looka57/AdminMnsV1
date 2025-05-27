// AdminMnsV1.Application.Services/Implementation/CandidatureService.cs
using AdminMnsV1.Application.Services.Interfaces; // Pour ICandidatureService, IDocumentService, etc.
using AdminMnsV1.Repositories.Interfaces; // Pour ICandidatureRepository, IUserRepository, IDocumentRepository, IDocumentTypeRepository
using AdminMnsV1.Models.Candidature;
using AdminMnsV1.Models.Documents; // Pour Document et DocumentType
using AdminMnsV1.Models; // Pour User
using AdminMnsV1.Models.ViewModels; // Pour CreateCandidatureViewModel
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AdminMnsV1.Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering; // Pour les includes
using AdminMnsV1.Models.Classes;
using AdminMnsV1.Repositories.Implementation;
using Microsoft.AspNetCore.Identity;
using AdminMnsV1.Models.Students;
using System.Web;
using System.Net; // Pour WebUtility.UrlEncode
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.WebUtilities;
using System.Text; // Ajoutez cette ligne en haut de CandidatureService.cs

namespace AdminMnsV1.Application.Services.Implementation // <-- TRÈS IMPORTANT : CORRESPOND AU USING DANS PROGRAM.CS
{
    public class CandidatureService : ICandidatureService
    {
        private readonly ICandidatureRepository _candidatureRepository;
        private readonly IUserRepository _userRepository; // Pour créer l'utilisateur si nécessaire
        private readonly IDocumentRepository _documentRepository; // Pour créer les documents initiaux
        private readonly IDocumentTypeRepository _documentTypeRepository; // Pour obtenir les types de documents
        private readonly IGenericRepository<CandidatureStatus> _candidatureStatusRepository; // Pour obtenir les statuts
        private readonly IGenericRepository<SchoolClass> _classRepository; // Pour obtenir les classes
        private readonly UserManager<User> _userManager; // Injection de UserManager
        private readonly IEmailService _emailService;


        public CandidatureService(
            ICandidatureRepository candidatureRepository,
            IUserRepository userRepository,
            IDocumentRepository documentRepository,
            IDocumentTypeRepository documentTypeRepository,
            IGenericRepository<CandidatureStatus> candidatureStatusRepository,
            IGenericRepository<SchoolClass> classRepository,
            UserManager<User> userManager,
            IEmailService emailService) // Injection de IEmailService si nécessaire



        {
            _candidatureRepository = candidatureRepository;
            _userRepository = userRepository;
            _documentRepository = documentRepository;
            _documentTypeRepository = documentTypeRepository;
            _candidatureStatusRepository = candidatureStatusRepository;
            _classRepository = classRepository;
            _userManager = userManager;
            _emailService = emailService; // Injection de IEmailService

        }


        public async Task<CreateCandidatureViewModel> PrepareCreateCandidatureViewModelAsync()
        {
            var classes = await _classRepository.GetAllAsync(); // Récupère toutes les classes
            var documentsTypes = await _documentTypeRepository.GetAllAsync(); // Récupère tous les types de documents

            var viewModel = new CreateCandidatureViewModel
            {
                AvailableClasses = classes.Select(c => new SelectListItem
                {
                    Value = c.ClasseId.ToString(),
                    Text = c.NameClass
                }).ToList(),
                AllAvailableDocumentTypes = documentsTypes.ToList() // Passe tous les types de documents
            };
            return viewModel;
        }

        public async Task<bool> CreateCandidatureAsync(CreateCandidatureViewModel model)
        {
            string resetPasswordUrl = null; // Initialisez-la à null
            // 1. Vérifier si l'utilisateur existe déjà ou le créer
            var user = (await _userRepository.FindAsync(u => u.Email == model.Email)).FirstOrDefault();
            bool isNewUser = (user == null); // Indicateur pour savoir si un nouvel utilisateur a été créé
            
            if (isNewUser)
            {
                user = new Student
                {
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.PhoneNumber,
                    BirthDate = (DateTime)model.BirthDate,
                    Status = model.Statut, // Ceci reste pour votre statut interne "Candidat"
                    UserName = model.Email // IMPORTANT : Renseigner UserName pour Identity
                };

                var identityResult = await _userManager.CreateAsync(user);
                if (!identityResult.Succeeded)
                {
                    throw new InvalidOperationException($"Échec de la création de l'utilisateur : {string.Join(", ", identityResult.Errors.Select(e => e.Description))}");
                }

                // --- C'est ici que l'on génère le token et affecte la variable resetPasswordUrl ---
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token)); // Convertissez le token en octets avant l'encodage
                // Affectez la variable déclarée plus haut
                resetPasswordUrl = $"https://localhost:7014/Identity/Account/ResetPassword?userId={user.Id}&token={encodedToken}";

                // Attribuer le rôle ASP.NET Identity "Student"
                if (!(await _userManager.IsInRoleAsync(user, "Student")))
                {
                    var result = await _userManager.AddToRoleAsync(user, "Student");
                    if (!result.Succeeded)
                    {
                        throw new InvalidOperationException($"Échec d'attribution du rôle 'Student' à l'utilisateur : {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }

                // --- DÉPLACÉ ICI : ENVOI DE L'EMAIL DE RÉINITIALISATION DE MOT DE PASSE APRÈS LA CRÉATION DU COMPTE ---
                var subject = "Votre candidature a été pré-enregistrée ! Créez votre mot de passe.";
                var message = $"Bonjour {model.FirstName} {model.LastName},<br/><br/>" +
                              "Votre candidature a été pré-enregistrée avec succès. <br/>" +
                              $"Veuillez cliquer sur le lien ci-dessous pour définir votre mot de passe et vous connecter à l'application et y déposer vos documents :<br/><br/>" +
                              
                              $"<a href=\"{resetPasswordUrl}\">Définir mon mot de passe</a><br/><br/>" + // ICI resetPasswordUrl est accessible
                              "Ce lien est valide pour une durée limitée.<br/><br/>" +
                              "Cordialement,<br/>L'équipe AdminMnsV1";

                try
                {
                    Console.WriteLine($"Tentative d'envoi d'e-mail à : {model.Email} avec lien de réinitialisation.");
                    await _emailService.SendEmailAsync(model.Email, subject, message);
                    Console.WriteLine($"Email de réinitialisation envoyé à {model.Email}.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Une erreur inattendue est survenue lors de l'envoi de l'e-mail de réinitialisation: {ex.Message}");
                    throw;
                }
            }
            // else: Si l'utilisateur existe déjà, vous n'envoyez pas d'e-mail de création/réinitialisation ici.

            // 2. Récupérer le statut "En cours"
            // ... (le reste du code est inchangé par rapport à ma suggestion précédente) ...

            var enCoursStatus = (await _candidatureStatusRepository.FindAsync(s => s.Label == "En cours")).FirstOrDefault();
            if (enCoursStatus == null)
            {
                throw new InvalidOperationException("Le statut 'En cours' n'existe pas dans la base de données.");
            }

            // 3. Créer la candidature
            var candidature = new Candidature
            {
                UserId = user.Id,
                ClassId = model.ClassId,
                CandidatureCreationDate = DateTime.Now,
                CandidatureStatusId = enCoursStatus.CandidatureStatusId,
                Progress = 0
            };
            _candidatureRepository.Add(candidature);
            var saved = await _candidatureRepository.SaveChangesAsync();

            if (saved > 0)
            {
                // 4. Créer les documents initialement requis avec le statut "Demandé"
                if (model.RequiredDocumentTypeIds != null && model.RequiredDocumentTypeIds.Any())
                {
                    var demandedDocumentStatusId = await _documentRepository.GetDocumentStatusIdByName("En cours");
                    if (demandedDocumentStatusId == null)
                    {
                        throw new InvalidOperationException("Le statut de document 'En cours' n'existe pas dans la base de données.");
                    }

                    foreach (var docTypeId in model.RequiredDocumentTypeIds)
                    {
                        var documentType = await _documentTypeRepository.GetByIdAsync(docTypeId);
                        if (documentType != null)
                        {
                            var document = new Documents
                            {
                                CandidatureId = candidature.CandidatureId,
                                DocumentTypeId = docTypeId,
                                StudentId = user.Id,
                                DocumentStatusId = (int)demandedDocumentStatusId,
                                DocumentPath = "N/A",
                                DocumentName = $"Document pour {documentType.NameDocumentType}",
                                DocumentDepositDate = DateTime.Now
                            };
                            _documentRepository.Add(document);
                        }
                    }

                    try
                    {
                        var savedDocumentsCount = await _documentRepository.SaveChangesAsync();
                        Console.WriteLine($"Tentative de sauvegarde de documents. Nombre de documents sauvegardés : {savedDocumentsCount}");
                    }
                    catch (DbUpdateException ex)
                    {
                        Console.WriteLine($"Erreur DbUpdateException lors de la sauvegarde des documents: {ex.Message}");
                        if (ex.InnerException != null)
                        {
                            Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                        }
                        throw;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Une erreur inattendue est survenue lors de la sauvegarde des documents: {ex.Message}");
                        throw;
                    }
                }
                return true;
            }
            return false;
        }
        public async Task<IEnumerable<Candidature>> GetAllCandidaturesWithDetailsAsync()
        {
            return await _candidatureRepository.GetAllCandidaturesWithDetailsAsync();
        }

        public async Task<Candidature?> GetCandidatureByIdWithDetailsAsync(int id)
        {
            return await _candidatureRepository.GetCandidatureByIdWithDetailsAsync(id);
        }

        public async Task<int?> GetCandidatureStatusIdByName(string statusName)
        {
            var status = (await _candidatureStatusRepository.FindAsync(s => s.Label == statusName)).FirstOrDefault();
            return status?.CandidatureStatusId;
        }

        public async Task<bool> UpdateCandidatureAsync(Candidature candidature)
        {
            _candidatureRepository.Update(candidature);
            return await _candidatureRepository.SaveChangesAsync() > 0;
        }
    }
}