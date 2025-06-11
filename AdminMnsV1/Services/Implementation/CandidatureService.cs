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
using AdminMnsV1.Application.Services.Interfaces;
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
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering; // Pour les includes
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;


// ...


namespace AdminMnsV1.Application.Services.Implementation
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
            var classes = await _classRepository.GetAllAbsenceAsync(); // Récupère toutes les classes
            var documentsTypes = await _documentTypeRepository.GetAllAbsenceAsync(); // Récupère tous les types de documents

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
            string resetPasswordUrl = null;
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
                    UserName = model.Email
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
                                DocumentPath = "",
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
                                 .Include(c => c.Documents)
                                 .Include(c => c.CandidatureStatus)
                                 .FirstOrDefaultAsync(c => c.UserId == userId);
        }




        public async Task<Candidature> GetCandidatureByIdWithDetailsAsync(int candidatureId)
        {
            return await _candidatureRepository.GetCandidatureByIdWithDetailsAsync(candidatureId);
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
            // !!! TRÈS IMPORTANT : Votre méthode _candidatureRepository.GetCandidatureByIdWithDetailsAsync(id)
            // DOIT inclure les navigations suivantes pour que les données soient disponibles :
            // .Include(c => c.User)
            // .Include(c => c.Class)
            // .Include(c => c.Documents).ThenInclude(d => d.DocumentType) // <-- C'EST CRITIQUE
            var candidature = await _candidatureRepository.GetCandidatureByIdWithDetailsAsync(id);

            // Si la candidature n'existe pas, retourne null
            if (candidature == null)
            {
                return null;
            }

            // 2. Calculer la progression du dossier (nombre de documents soumis/vérifiés)
            // totalRequiredDocs doit être le nombre total de TYPES de documents attendus dans le système.
            // Cela nécessite une requête sur votre DbContext (_context).
            // Nous comptons tous les types de documents définis dans la base de données.
            int totalRequiredDocs = await _context.DocumentTypes.CountAsync();

            // submittedDocs et verifiedDocs utilisent la collection de DOCUMENTS SOUMIS par la candidature.
            // C'EST ICI LA CORRECTION PRINCIPALE : Utilisation de 'candidature.Documents' qui est la bonne propriété.
            int submittedDocs = candidature.Documents?.Count(d => !string.IsNullOrEmpty(d.DocumentPath)) ?? 0;
            int verifiedDocs = candidature.Documents?.Count(d => d.IsVerified) ?? 0;

            int studentProgress = candidature.StudentValidationProgress;
            int mnsProgress = candidature.MnsValidationProgress;

            // 3. Mapper les données de l'entité Candidature vers le ViewModel CandidatureStudentViewModel
            var viewModel = new CandidatureStudentViewModel
            {
                CandidatureId = candidature.CandidatureId,
                CandidatureStatus = candidature.CandidatureStatus?.Label ?? "Statut inconnu",
                FirstName = candidature.User?.FirstName ?? "N/A", // Ajout de "N/A" si la valeur est null
                LastName = candidature.User?.LastName ?? "N/A",
                Email = candidature.User?.Email ?? "N/A",
                Phone = candidature.User?.Phone ?? "N/A", // Utilise Phone au lieu de PhoneNumber si c'est le nom de la propriété
                Address = candidature.User?.Address ?? "N/A",
                BirthDate = candidature.User?.BirthDate,
                ClassName = candidature.Class?.NameClass ?? "N/A", // Ajout de '?' et "N/A" 
                CandidatureStatuses = candidature.CandidatureStatus?.Label, // Utilisez ?. pour gérer le cas où CandidatureStatus serait null

                StudentValidationProgress = studentProgress,
                MnsValidationProgress = mnsProgress,
                StudentImage = string.IsNullOrEmpty(candidature.User?.Photo)
                    ? "/images/logo/defaut.png" // Chemin par défaut si Photo est null ou vide
                    : "/images/profiles/" + candidature.User.Photo, // Concaténation du chemin du dossier avec le nom du fichier

                // 4. Mapper les documents requis pour l'affichage dans le ViewModel.
                // Cette section va afficher TOUS les types de documents attendus,
                // et pour chacun, indiquer si l'étudiant l'a soumis et si c'est vérifié.
                RequiredDocuments = candidature.Documents? // Utilisez les documents réellement liés à cette candidature
            .Select(d => new DocumentViewModel
            {
                DocumentId = d.DocumentId,
                DocumentName = d.DocumentName ?? d.DocumentType?.NameDocumentType ?? "Nom Document Non Défini",
                DocumentTypeName = d.DocumentType?.NameDocumentType ?? "Type de Document Inconnu",
                UploadDate = d.DocumentDepositDate,
                DocumentPath = (d.DocumentPath != null && !string.Equals(d.DocumentPath, "/uploads/documents/N/A", StringComparison.OrdinalIgnoreCase))
                               ? $"/uploads/documents/{d.DocumentPath}"
                               : null,
                IsVerified = d.IsVerified
            }).ToList() ?? new List<DocumentViewModel>() // Gérer le cas où candidature.Documents est null
            };

            return viewModel;
        }
        public async Task<IEnumerable<CandidatureStudentViewModel>> GetAllCandidaturesForOverviewAsync()
        {

            var allCandidatures = await _candidatureRepository.GetAllCandidaturesWithDetailsAsync();


            //var allDocumentTypes = await _documentTypeRepository.GetAllAsync();
            //var totalRequiredDocumentTypesCount = allDocumentTypes.Count();

            // 3. Mapper les entités Candidature vers une liste de CandidatureStudentViewModel
            var viewModels = allCandidatures
                .Where(c => c.User != null && !c.User.IsDeleted)
                .Select(c =>
                {
                    // Initialisation du ViewModel
                    var vm = new CandidatureStudentViewModel
                    {
                        CandidatureId = c.CandidatureId,
                        FirstName = c.User?.FirstName,
                        LastName = c.User?.LastName,
                        Email = c.User?.Email,
                        Phone = c.User?.PhoneNumber,
                        Address = c.User?.Address,
                        BirthDate = c.User?.BirthDate,
                        ClassName = c.Class?.NameClass,


                        CandidatureStatuses = c.CandidatureStatus?.Label,

                        // Initialisation des progressions à 0 avant le calcul détaillé
                        // C'est correct, car elles seront mises à jour juste après.
                        StudentValidationProgress = c.StudentValidationProgress,
                        MnsValidationProgress = c.MnsValidationProgress,

                        // Mapper les documents liés à cette candidature
                        RequiredDocuments = c.Documents?.Select(d => new DocumentViewModel
                        {
                            DocumentId = d.DocumentId,
                            DocumentName = d.DocumentName, // Nom du document uploadé (ex: "mon_cv.pdf")
                            DocumentTypeName = d.DocumentType?.NameDocumentType, // Nom du type de document requis (ex: "Curriculum Vitae")
                            DocumentPath = (d.DocumentPath != null && !string.Equals(d.DocumentPath, "/uploads/documents/N/A", StringComparison.OrdinalIgnoreCase))
                                   ? $"/uploads/documents/{d.DocumentPath}"
                                   : null, // Ou string.Empty si vous préférez afficher une chaîne vide
                            IsVerified = d.IsVerified
                        }).ToList() ?? new List<DocumentViewModel>()
                    };

                    // 4. Calcul des progressions pour CHAQUE ViewModel (candidature)
                    // Ce bloc est exécuté pour chaque `vm` après son initialisation.
                    //if (totalRequiredDocumentTypesCount > 0)
                    //{
                    //    // Progression étudiant : basées sur les documents uploadés
                    //    // `vm.RequiredDocuments` contient uniquement les documents que cette candidature a.
                    //    // Il faut s'assurer que `c.DocumentTypes` ramène les documents *liés* à cette candidature
                    //    // et non pas tous les types de documents.
                    //    var uploadedDocumentsCount = vm.RequiredDocuments.Count(d => !string.IsNullOrEmpty(d.DocumentPath));
                    //    vm.StudentValidationProgress = (int)(((double)uploadedDocumentsCount / totalRequiredDocumentTypesCount) * 100);

                    //    // Progression MNS : basées sur les documents vérifiés
                    //    var verifiedDocumentsCount = vm.RequiredDocuments.Count(d => d.IsVerified);
                    //    vm.MnsValidationProgress = (int)(((double)verifiedDocumentsCount / totalRequiredDocumentTypesCount) * 100);
                    //}
                    //else
                    //{
                    //    // Si aucun type de document n'est défini comme requis, la progression est 0.
                    //    // (Ou 100 si vous considérez qu'il n'y a rien à faire) - 0 est plus sûr ici.
                    //    vm.StudentValidationProgress = 0;
                    //    vm.MnsValidationProgress = 0;
                    //}

                    return vm; // Retourne le ViewModel complété pour cette candidature
                })
                .ToList(); // Convertit le résultat du Select en List<CandidatureStudentViewModel>

            return viewModels;
        }



        public async Task<bool> UploadDocumentAsync(int candidatureId, IFormFile document, string documentTypeName)
        {

            if (string.IsNullOrWhiteSpace(documentTypeName))
            {
                Console.WriteLine("Erreur : Le nom du type de document est null ou vide.");
                return false;
            }
            // Récupérer la candidature et le type de document
            var candidature = await _candidatureRepository.GetCandidatureByIdWithDetailsAsync(candidatureId);
            if (candidature == null)
            {
                return false;
            }

            //le document à uploader.
            var cleanedAndLoweredDocumentTypeName = documentTypeName.Trim().ToLower();
            var documentType = await _context.DocumentTypes
                                             .FirstOrDefaultAsync(dt => dt.NameDocumentType.Trim().ToLower() == cleanedAndLoweredDocumentTypeName);


            if (documentType == null)
            {
                // Aucun document à mettre à jour, ou tous les documents requis ont déjà un chemin
                return false;
            }

            // Traiter le fichier 
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "documents");
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

                // Mettre à jour ou créer l'entrée de document dans la DB
                var existingDocument = await _context.Documents.FirstOrDefaultAsync(d => d.CandidatureId == candidatureId && d.DocumentTypeId == documentType.DocumentTypeId);

                if (existingDocument != null)
                {
                    existingDocument.DocumentPath = uniqueFileName; // Chemin relatif pour la DB
                    existingDocument.DocumentDepositDate = DateTime.Now;
                    existingDocument.IsVerified = false; // Par défaut, un nouveau document n'est pas vérifié
                }
                else
                {
                    // Création d'un nouveau document si aucun n'existe pour ce type et cette candidature
                    var newDocument = new Documents
                    {
                        CandidatureId = candidatureId,
                        DocumentTypeId = documentType.DocumentTypeId,
                        DocumentName = document.FileName, // Nom original du fichier
                        DocumentPath = uniqueFileName,
                        DocumentDepositDate = DateTime.Now,
                        IsVerified = false // Par défaut, un nouveau document est en attente de validation
                    };
                    _context.Documents.Add(newDocument);
                }
                // Mettre à jour les barres de progression 
                // Fait après l'enregistrement des modifications.

                await _context.SaveChangesAsync();

                // Calculer et mettre à jour la progression de l'étudiant
                await CalculateAndSaveCandidatureProgresses(candidatureId);


                return true;
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'upload du document : {ex.Message}");
                return false;
            }
        }

        // REMPLACEZ VOS DEUX MÉTHODES UpdateStudentValidationProgress ET UpdateMnsValidationProgress PAR CELLE-CI

        private async Task CalculateAndSaveCandidatureProgresses(int candidatureId)
        {
            var candidature = await _context.Candidatures
                .Include(c => c.Documents)
                .FirstOrDefaultAsync(c => c.CandidatureId == candidatureId);

            if (candidature == null)
            {
                Console.WriteLine($"DEBUG: Candidature {candidatureId} non trouvée. Impossible de calculer la progression.");
                return;
            }

            // Le dénominateur est le nombre total de "slots" de documents créés pour cette candidature.
            // C'est ce que vous insérez dans CreateCandidatureAsync.
            var totalExpectedDocumentsCount = candidature.Documents.Count; // C'EST LA LIGNE CORRECTE !

            if (totalExpectedDocumentsCount > 0)
            {
                // Calcul de la progression étudiant (documents uploadés)
                var uploadedDocsCount = candidature.Documents.Count(d => !string.IsNullOrWhiteSpace(d.DocumentPath));
                candidature.StudentValidationProgress = (int)Math.Round((double)uploadedDocsCount / totalExpectedDocumentsCount * 100);

                // Calcul de la progression MNS (documents vérifiés et uploadés)
                var verifiedDocsCount = candidature.Documents.Count(d => d.IsVerified && !string.IsNullOrWhiteSpace(d.DocumentPath));
                candidature.MnsValidationProgress = (int)Math.Round((double)verifiedDocsCount / totalExpectedDocumentsCount * 100);
            }
            else
            {
                // Aucun document requis, donc progression à 0 (ou 100 si c'est votre règle)
                candidature.StudentValidationProgress = 0;
                candidature.MnsValidationProgress = 0;
            }

            // Sauvegarder les modifications dans la base de données
            try
            {
                await _context.SaveChangesAsync();
                Console.WriteLine($"Progression de la candidature {candidatureId} mise à jour et sauvegardée en DB.");
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Erreur DbUpdateException lors de la sauvegarde des progressions: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                throw; // Relancer l'exception pour la gestion des erreurs
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Une erreur inattendue est survenue lors de la sauvegarde des progressions: {ex.Message}");
                throw;
            }
        }


        // Dans CandidatureService.cs
        public async Task<int> ValidateDocumentAsync(int documentId, string adminUserId)
        {
            var document = await _context.Documents.FindAsync(documentId);
            if (document == null)
            {
                return 0;
            }

            document.IsVerified = true;
            document.AdminId = adminUserId;
            document.ValidationDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            await CalculateAndSaveCandidatureProgresses(document.CandidatureId); 
            return document.CandidatureId;
         
        }

        public async Task<int> RejectDocumentAsync(int documentId, string adminUserId)
        {
            var document = await _context.Documents.FindAsync(documentId);
            if (document == null) return 0;

            document.IsVerified = false; // Rejeter (ou marquer comme non validé)
            document.AdminId = adminUserId;
            document.ValidationDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            await CalculateAndSaveCandidatureProgresses(document.CandidatureId);
            return document.CandidatureId;
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<bool> DeleteCandidatureAsync(int id)
        {
            var candidature = await _candidatureRepository.GetCandidatureByIdWithDetailsAsync(id); // Utilisez la méthode qui inclut les documents
            if (candidature == null) return false;

            // Optionnel : supprimer les fichiers physiques associés aux documents de la candidature
            if (candidature.Documents != null && candidature.Documents.Any())
            {
                // Supprimer les fichiers physiques associés aux documents de la candidature
                foreach (var doc in candidature.Documents.Where(d => !string.IsNullOrEmpty(d.DocumentPath)))
                {
                    var filePath = Path.Combine(_webHostEnvironment.WebRootPath, doc.DocumentPath.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }
            }
            _context.Documents.RemoveRange(candidature.Documents);


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

        public async Task<CandidatureStudentViewModel> GetCandidatureDetailsByUserIdAsync(string userId)
        {
            var candidature = await _context.Candidatures
                .Include(c => c.User)
                .Include(c => c.Class)
                .Include(c => c.CandidatureStatus) // Inclure le statut de la candidature
                .Include(c => c.Documents)       // Inclure les documents liés à la candidature
                    .ThenInclude(d => d.DocumentType) // Et inclure le type de document pour chaque document
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (candidature == null)
            {
                return null;
            }

            // --- Étape 1 : Récupérer le nombre total de types de documents requis pour la progression ---
            // Cette partie est CRUCIALE. C'est le dénominateur de votre calcul de pourcentage.
            // Assurez-vous que _documentTypeRepository.GetAllAsync() renvoie bien TOUS les types de documents
            // qu'un étudiant DOIT potentiellement fournir pour une candidature complète.
            //var allRequiredDocumentTypes = await _documentTypeRepository.GetAllAsync(); // Assurez-vous que ce repository est injecté.
            //var totalRequiredDocumentTypesCount = allRequiredDocumentTypes.Count();

            // --- Étape 2 : Mapper l'entité Candidature vers le ViewModel ---
            var viewModel = new CandidatureStudentViewModel
            {
                CandidatureId = candidature.CandidatureId,
                // Assurez-vous que 'CandidatureStatuses.Label' est le bon nom et que 'CandidatureStatuses' est bien chargé.
                CandidatureStatus = candidature.CandidatureStatus?.Label,
                FirstName = candidature.User?.FirstName ?? "N/A",
                LastName = candidature.User?.LastName ?? "N/A",
                Email = candidature.User?.Email ?? "N/A",
                Phone = candidature.User?.PhoneNumber ?? "N/A",
                Address = candidature.User?.Address ?? "N/A",
                BirthDate = candidature.User?.BirthDate,
                // Correction pour StudentImage pour correspondre à votre logique de chemin
                StudentImage = string.IsNullOrEmpty(candidature.User?.Photo)
                                     ? "/images/profiles/default_student.png"
                                     : "/images/profiles/" + candidature.User.Photo,

                ClassName = candidature.Class?.NameClass ?? "N/A",
                ClassStartDate = candidature.Class?.StartDate ?? default,
                ClassEndDate = candidature.Class?.EndDate ?? default,

                // --- Étape 3 : Calculer les pourcentages de progression ICI ---
                // Ne mettez PAS de valeurs en dur. Calculez-les dynamiquement.
                StudentValidationProgress = 0, // Initialiser à 0, puis calculer
                MnsValidationProgress = 0,     // Initialiser à 0, puis calculer

                RequiredDocuments = candidature.Documents?.Select(d => new DocumentViewModel
                {
                    DocumentId = d.DocumentId,
                    DocumentName = d.DocumentName,
                    DocumentTypeName = d.DocumentType?.NameDocumentType ?? "Non défini",
                    UploadDate = (d.DocumentPath != null && !string.Equals(d.DocumentPath, "N/A", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(d.DocumentPath))
                 ? (d.DocumentDepositDate != null ? d.DocumentDepositDate : DateTime.MinValue) // Utilisation de .Value pour DateTime? ou directement si DateTime
                 : DateTime.MinValue, // Si pas de document déposé (chemin invalide), la date est DateTime.MinValue

                    // Ajustement du chemin du document : utilisez l'interpolation de chaîne si DocumentPath est juste le nom du fichier.
                    DocumentPath = !string.IsNullOrEmpty(d.DocumentPath) ? $"/uploads/documents/{d.DocumentPath}" : null,
                    IsVerified = d.IsVerified
                }).ToList() ?? new List<DocumentViewModel>()
            };

            // Le total des documents attendus est le nombre de documents dans la candidature elle-même.
            // Chaque document dans 'candidature.Documents' représente un slot pour un document.
            var totalExpectedDocumentsCount = viewModel.RequiredDocuments.Count;


            // --- Étape 4 : Appliquer la logique de calcul de la progression ---
            // Cette partie est exécutée APRÈS que le ViewModel est créé et que sa liste RequiredDocuments est remplie.
            if (totalExpectedDocumentsCount > 0)
            {
                // Progression de l'étudiant : documents téléchargés
                var uploadedDocsCount = viewModel.RequiredDocuments.Count(d => !string.IsNullOrEmpty(d.DocumentPath));
                viewModel.StudentValidationProgress = (int)Math.Round(((double)uploadedDocsCount / totalExpectedDocumentsCount) * 100);

                // Progression MNS : documents vérifiés
                var verifiedDocsCount = viewModel.RequiredDocuments.Count(d => d.IsVerified);
                viewModel.MnsValidationProgress = (int)Math.Round(((double)verifiedDocsCount / totalExpectedDocumentsCount) * 100);
            }
            else
            {
                // Si la candidature n'a pas de documents associés du tout, la progression est 0.
                viewModel.StudentValidationProgress = 0;
                viewModel.MnsValidationProgress = 0;
            }

            return viewModel;
        }


        public async Task UpdateCandidatureStatusBasedOnDocuments(int candidatureId)
        {
            var candidature = await _candidatureRepository.GetCandidatureByIdWithDetailsAsync(candidatureId);
            if (candidature == null)
            {
                return;
            }
            // Récupérer le nombre total de types de documents requis
            int totalRequireDocs = candidature.Documents?.Count() ?? 0; // Comptez les documents associés à cette candidature.
            // Récupérer le nombre de documents soumis et validés pour cette candidature
            int verifiedDocs = candidature.Documents?.Count(d => d.IsVerified) ?? 0;
            // Vérifier si tous les documents requis sont validés
            if (totalRequireDocs >0 && verifiedDocs == totalRequireDocs)
            {
                // Trouver l'ID du statut "Validé"
                var validatedStatus = await _context.CandidatureStatuses
                                                             .FirstOrDefaultAsync(s => s.Label == "Validé");
                if (validatedStatus != null && candidature.CandidatureStatusId != validatedStatus.CandidatureStatusId)
                {
                    // Mettre à jour le statut de la candidature
                    candidature.CandidatureStatusId = validatedStatus.CandidatureStatusId;
                    _context.Candidatures.Update(candidature);
                    await _context.SaveChangesAsync();

                    // >>> Appeler la logique pour mettre à jour le rôle de l'utilisateur ici <<<
                    // Nous allons créer cette méthode séparément pour la clarté.
                   await UpdateUserStatusToStudent(candidature.UserId);
                }
            }
        }

        // Dans CandidatureService.cs
        private async Task UpdateUserStatusToStudent(string userId) // Renommé pour plus de clarté
        {
            var user = await _userManager.FindByIdAsync(userId); // _userManager est toujours utile pour trouver l'utilisateur
            if (user == null)
            {
                return;
            }

            // Vérifier si le statut actuel de l'utilisateur est "Candidat"
            // Adaptez "Status" et "Candidat" au nom exact de votre propriété et de la valeur du statut
            if (user.Status == "Candidat")
            {
                user.Status = "Stagiaire"; // Mettre à jour le statut
                var updateResult = await _userManager.UpdateAsync(user); // Utiliser UpdateAsync pour sauvegarder les changements sur l'utilisateur
                if (!updateResult.Succeeded)
                {
                    // Gérer l'erreur si la mise à jour échoue
                    // (Ex: logger les erreurs dans updateResult.Errors)
                }
            }
          
        }











        public Task<bool> UploadDocumentAsync(int candidatureId, IFormFile document, object documentTypeName)
        {
            throw new NotImplementedException();
        }
    }
}
