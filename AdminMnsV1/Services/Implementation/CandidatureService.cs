// AdminMnsV1.Application.Services/Implementation/CandidatureService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net; // Pour WebUtility.UrlEncode
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System.Web;
using AdminMnsV1.Application.Services.Interfaces; // Pour ICandidatureService, IDocumentService, etc.
using AdminMnsV1.Data;
using AdminMnsV1.Data.Repositories.Implementation;
using AdminMnsV1.Data.Repositories.Interfaces;
using AdminMnsV1.Models; // Pour User
using AdminMnsV1.Models.CandidaturesModels;
using AdminMnsV1.Models.Classes;
using AdminMnsV1.Models.Documents; // Pour Document et DocumentType
using AdminMnsV1.Models.Students;
using AdminMnsV1.Models.ViewModels; // Pour CreateCandidatureViewModel
using AdminMnsV1.Repositories.Implementation;
using AdminMnsV1.Repositories.Interfaces; // Pour ICandidatureRepository, IUserRepository, IDocumentRepository, IDocumentTypeRepository
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering; // Pour les includes
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;

// ...


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
        private readonly IGenericRepository<Attend> _attendRepository;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment; // Ajout de l'environnement web





        public CandidatureService(
            ICandidatureRepository candidatureRepository,
            IUserRepository userRepository,
            IDocumentRepository documentRepository,
            IDocumentTypeRepository documentTypeRepository,
            IGenericRepository<CandidatureStatus> candidatureStatusRepository,
            IGenericRepository<SchoolClass> classRepository,
            UserManager<User> userManager,
            IEmailService emailService,
            IGenericRepository<Attend> attendRepository,
            ApplicationDbContext context, // Ajout de ApplicationDbContext pour les opérations de base de données
            IWebHostEnvironment webHostEnvironment) // Injection de IWebHostEnvironment


        {
            _candidatureRepository = candidatureRepository;
            _userRepository = userRepository;
            _documentRepository = documentRepository;
            _documentTypeRepository = documentTypeRepository;
            _candidatureStatusRepository = candidatureStatusRepository;
            _classRepository = classRepository;
            _userManager = userManager;
            _emailService = emailService;
            _attendRepository = attendRepository;
            _context = context;
            _webHostEnvironment = webHostEnvironment; // Initialisation

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
                    Phone = model.Phone,
                    BirthDate = (DateTime)model.BirthDate,
                    IsOnboardingCompleted = false, // TRÈS IMPORTANT : Doit être false au début
                    CreationDate = DateTime.UtcNow,
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

                var attendEntry = new Attend
                {
                    StudentId = user.Id,
                    ClasseId = model.ClassId // <<< Utilisez 'ClasseId' ici, en cohérence avec votre modèle Attend
                };
                _attendRepository.Add(attendEntry); // _studentClassRepository doit être pour l'entité Attend
                await _attendRepository.SaveChangesAsync(); // Sauvegarde la liaison stagiaire-classe


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

        // Dans CandidatureService.cs
public async Task<Candidature> GetCandidatureByUserIdAsync(string userId)
{
    return await _context.Candidatures
                         .Include(c => c.CandidatureStatus) 
                         .FirstOrDefaultAsync(c => c.UserId == userId);
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

        // Dans CandidatureService.cs
        public async Task<CandidatureStatus> GetCandidatureStatusByIdAsync(int statusId)
        {
            // Assurez-vous que vous retournez un objet CandidatureStatus, pas juste un int
            return await _context.CandidatureStatuses.FirstOrDefaultAsync(s => s.CandidatureStatusId == statusId);
        }

        public async Task<bool> UpdateCandidatureAsync(Candidature candidature)
        {
            _candidatureRepository.Update(candidature);
            return await _candidatureRepository.SaveChangesAsync() > 0;
        }


        public async Task<CandidatureStudentViewModel> GetCandidatureDetailsAsync(int id)
        {
            // 1. Récupérer la candidature avec tous les détails nécessaires depuis le repository
            var candidature = await _candidatureRepository.GetCandidatureByIdWithDetailsAsync(id);

            // Si la candidature n'existe pas, retourne null
            if (candidature == null)
            {
                return null;
            }

            // 2. Calculer la progression du dossier (nombre de documents soumis/vérifiés)
            int totalRequiredDocs = candidature.Documents?.Count ?? 0;
            int submittedDocs = candidature.Documents?.Count(d => !string.IsNullOrEmpty(d.DocumentPath)) ?? 0;
            int verifiedDocs = candidature.Documents?.Count(d => d.IsVerified) ?? 0;

            int studentProgress = totalRequiredDocs > 0 ? (int)((double)submittedDocs / totalRequiredDocs * 100) : 0;
            int mnsProgress = totalRequiredDocs > 0 ? (int)((double)verifiedDocs / totalRequiredDocs * 100) : 0;

            // 3. Mapper les données de l'entité Candidature vers le ViewModel CandidatureStudentViewModel
            var viewModel = new CandidatureStudentViewModel
            {
                CandidatureId = candidature.CandidatureId,
                CandidatureStatus = candidature.CandidatureStatus?.Label, // Assure-toi d'accéder au Label du statut
                FirstName = candidature.User?.FirstName,
                LastName = candidature.User?.LastName,
                Email = candidature.User?.Email,
                PhoneNumber = candidature.User?.Phone, // Utilise Phone au lieu de PhoneNumber si c'est le nom de la propriété
                Address = candidature.User?.Address,
                BirthDate = candidature.User?.BirthDate,
                ClassName = candidature.Class.NameClass, // Utilise SchoolClass au lieu de Class
                StudentValidationProgress = studentProgress,
                MnsValidationProgress = mnsProgress,
                StudentImage = "/images/default_student.png", // Chemin par défaut

                // 4. Mapper les documents requis
                RequiredDocuments = candidature.Documents.Select(cd => new DocumentViewModel
                {
                    DocumentId = cd.DocumentId,
                    DocumentTypeName = cd.DocumentType.NameDocumentType,
                    UploadDate = cd.DocumentDepositDate, // Utilise DocumentDepositDate
                    DocumentPath = cd.DocumentPath,
                    IsVerified = cd.IsVerified
                }).ToList() ?? new List<DocumentViewModel>(),

                // 5.  Notifications (exemple - à adapter si tu as une entité Notification)
                //Notifications = new List<NotificationViewModel>() // À implémenter si tu as des notifications
            };

            return viewModel;
        }

        public async Task<IEnumerable<CandidatureStudentViewModel>> GetAllCandidaturesForOverviewAsync()
        {
            var allCandidatures = await _candidatureRepository.GetAllCandidaturesWithDetailsAsync(); // Supposons que cette méthode ramène Candidature avec User, Class, CandidatureStatus, Documents inclus

            // Mapper les entités Candidature vers une liste de CandidatureStudentViewModel
            var viewModels = allCandidatures
                .Where(c => c.User != null && !c.User.IsDeleted) // Filtrez les utilisateurs non supprimés
                .Select(c => new CandidatureStudentViewModel
                {
                    CandidatureId = c.CandidatureId,
                    FirstName = c.User?.FirstName,
                    LastName = c.User?.LastName,
                    Email = c.User?.Email,
                    PhoneNumber = c.User?.PhoneNumber,
                    Address = c.User?.Address,
                    BirthDate = c.User?.BirthDate,
                    ClassName = c.Class?.NameClass,
                    CandidatureStatus = c.CandidatureStatus?.Label,
                    //StudentImage = c.User?.StudentImage, // Si vous avez cette propriété

                    // Le calcul des progressions peut être complexe ici,
                    // Si _documentTypeRepository.GetAllDocumentTypesAsync() est nécessaire,
                    // il faudrait le charger une seule fois avant la boucle ou le passer en paramètre.
                    // Pour simplifier temporairement:
                    StudentValidationProgress = 0, // À implémenter correctement
                    MnsValidationProgress = 0,     // À implémenter correctement

                    RequiredDocuments = c.Documents?.Select(d => new DocumentViewModel
                    {
                        DocumentId = d.DocumentId,
                        DocumentName = d.DocumentName,
                        DocumentTypeName = d.DocumentType?.NameDocumentType, 
                        DocumentPath = d.DocumentPath,
                        IsVerified = d.IsVerified // Assurez-vous toujours que cette propriété existe
                    }).ToList() ?? new List<DocumentViewModel>()

                }).ToList();

            // Note sur la progression: Pour calculer correctement StudentValidationProgress et MnsValidationProgress
            // ici, vous devrez peut-être récupérer le nombre total de types de documents requis
            // une seule fois avant la boucle .Select(), car appeler .Result.Count() à l'intérieur
            // d'un Select() peut être inefficace ou poser des problèmes.

            var totalRequiredDocumentTypesCount = _documentTypeRepository.GetAllAsync().Result.Count(); // Ou mieux, gérez-le de manière async.
            foreach (var vm in viewModels)
            {
                if (totalRequiredDocumentTypesCount > 0)
                {
                    vm.StudentValidationProgress = (int)(((double)vm.RequiredDocuments.Count(d => !string.IsNullOrEmpty(d.DocumentPath)) / totalRequiredDocumentTypesCount) * 100);
                    vm.MnsValidationProgress = (int)(((double)vm.RequiredDocuments.Count(d => d.IsVerified) / totalRequiredDocumentTypesCount) * 100);
                }
                else
                {
                    vm.StudentValidationProgress = 0;
                    vm.MnsValidationProgress = 0;
                }
            }


            return viewModels;
        }

        public async Task<bool> UploadDocumentAsync(int candidatureId, IFormFile document)
        {
            var candidature = await _candidatureRepository.GetCandidatureByIdWithDetailsAsync(candidatureId);
            if (candidature == null) return false;

            // Logique pour identifier le document à uploader.
            // Ceci est un exemple : tu devras peut-être passer un DocumentTypeId ou un DocumentId dans le formulaire.
            // Pour l'exemple, nous allons chercher un document qui n'a pas encore de chemin.
            var targetDocument = candidature.Documents.FirstOrDefault(d => string.IsNullOrEmpty(d.DocumentPath));

            if (targetDocument == null)
            {
                // Aucun document à mettre à jour, ou tous les documents requis ont déjà un chemin
                return false;
            }

            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "documents");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + document.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            try
            {
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await document.CopyToAsync(fileStream);
                }

                targetDocument.DocumentPath = "/documents/" + uniqueFileName; // Chemin relatif pour la DB
                targetDocument.DocumentDepositDate = DateTime.Now;
                targetDocument.IsVerified = false; // Par défaut, un nouveau document n'est pas vérifié

                _documentRepository.Update(targetDocument); // Met à jour l'état du document dans le contexte
                await _documentRepository.SaveChangesAsync(); // Enregistre les changements en base de données

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'upload du document : {ex.Message}");
                // Gérer l'erreur, peut-être logguer ou renvoyer un statut spécifique
                return false;
            }
        }
        public async Task<int> ValidateDocumentAsync(int documentId)
        {
            var documentToUpdate = await _documentRepository.GetByIdAsync(documentId);
            if (documentToUpdate == null) return 0; // Document non trouvé

            documentToUpdate.IsVerified = true;
            _documentRepository.Update(documentToUpdate);
            await _documentRepository.SaveChangesAsync();

            // Retourne l'ID de la candidature associée au document pour la redirection ou le rafraîchissement
            return documentToUpdate.CandidatureId;
        }
        public async Task<bool> DeleteCandidatureAsync(int id)
        {
            var candidature = await _candidatureRepository.GetByIdAsync(id);
            if (candidature == null) return false;

            // Optionnel : supprimer les fichiers physiques associés aux documents de la candidature
            if (candidature.Documents != null)
            {
                foreach (var doc in candidature.Documents.Where(d => !string.IsNullOrEmpty(d.DocumentPath)))
                {
                    var filePath = Path.Combine(_webHostEnvironment.WebRootPath, doc.DocumentPath.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }
            }

            _candidatureRepository.Delete(candidature);
            return await _candidatureRepository.SaveChangesAsync() > 0;
        }


        public async Task<bool> UpdateCandidatureStatusAsync(int candidatureId, string newStatusName)
        {
            var candidature = await _candidatureRepository.GetByIdAsync(candidatureId);
            if (candidature == null) return false;

            var newStatus = (await _candidatureStatusRepository.FindAsync(s => s.Label == newStatusName)).FirstOrDefault();
            if (newStatus == null) return false;

            candidature.CandidatureStatusId = newStatus.CandidatureStatusId;
            // Si tu as une propriété string 'Statut' dans Candidature, mets-la à jour aussi si besoin:
            // candidature.Statut = newStatusName;

            _candidatureRepository.Update(candidature);
            return await _candidatureRepository.SaveChangesAsync() > 0;
        }



        public async Task<int> RejectDocumentAsync(int documentId)
        {
            var documentToUpdate = await _documentRepository.GetByIdAsync(documentId);
            if (documentToUpdate == null) return 0; // Document non trouvé

            documentToUpdate.IsVerified = false; // Marque comme non vérifié ou rejeté
            // Optionnel : tu peux réinitialiser le chemin si l'étudiant doit le soumettre à nouveau
            // documentToUpdate.DocumentPath = null;
            // documentToUpdate.DocumentDepositDate = null;

            _documentRepository.Update(documentToUpdate);
            await _documentRepository.SaveChangesAsync();

            // Retourne l'ID de la candidature associée au document
            return documentToUpdate.CandidatureId;
        }

    }
}