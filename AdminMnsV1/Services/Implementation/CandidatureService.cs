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
using AdminMnsV1.Application.Services.Interfaces;


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
                                 .Include(c => c.CandidatureStatuses)
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
            int submittedDocs = candidature.DocumentTypes?.Count(d => !string.IsNullOrEmpty(d.DocumentPath)) ?? 0;
            int verifiedDocs = candidature.DocumentTypes?.Count(d => d.IsVerified) ?? 0;

            int studentProgress = totalRequiredDocs > 0 ? (int)((double)submittedDocs / totalRequiredDocs * 100) : 0;
            int mnsProgress = totalRequiredDocs > 0 ? (int)((double)verifiedDocs / totalRequiredDocs * 100) : 0;

            // 3. Mapper les données de l'entité Candidature vers le ViewModel CandidatureStudentViewModel
            var viewModel = new CandidatureStudentViewModel
            {
                CandidatureId = candidature.CandidatureId,
                CandidatureStatus = candidature.CandidatureStatuses.Label, // Votre CandidatureStatus est un string directement
                FirstName = candidature.User?.FirstName ?? "N/A", // Ajout de "N/A" si la valeur est null
                LastName = candidature.User?.LastName ?? "N/A",
                Email = candidature.User?.Email ?? "N/A",
                Phone = candidature.User?.Phone ?? "N/A", // Utilise Phone au lieu de PhoneNumber si c'est le nom de la propriété
                Address = candidature.User?.Address ?? "N/A",
                BirthDate = candidature.User?.BirthDate,
                ClassName = candidature.Class?.NameClass ?? "N/A", // Ajout de '?' et "N/A" au cas où Class serait null
                StudentValidationProgress = studentProgress,
                MnsValidationProgress = mnsProgress,
                StudentImage = string.IsNullOrEmpty(candidature.User?.Photo)
                    ? "/images/logo/defaut.png" // Chemin par défaut si Photo est null ou vide
                    : "/images/profiles/" + candidature.User.Photo, // Concaténation du chemin du dossier avec le nom du fichier

                // 4. Mapper les documents requis pour l'affichage dans le ViewModel.
                // Cette section va afficher TOUS les types de documents attendus,
                // et pour chacun, indiquer si l'étudiant l'a soumis et si c'est vérifié.
                RequiredDocuments = (await _context.DocumentTypes.ToListAsync()) // Récupère TOUS les types de documents existants de la DB
                    .Select(documentType =>
                    { // 'documentType' ici est un objet DocumentType (ex: "CV", "RIB")
                      // On cherche si un document SOUMIS par CETTE candidature correspond à CE type de document.
                        var submittedDocument = candidature.DocumentTypes?.FirstOrDefault(d => d.DocumentTypeId == documentType.DocumentTypeId);

                        return new DocumentViewModel
                        {
                            DocumentId = submittedDocument?.DocumentId ?? 0, // ID du document soumis, ou 0 si non soumis
                                                                             // Si le document est soumis, on prend son nom de fichier, sinon on prend le nom du type de document
                            DocumentName = submittedDocument?.DocumentName ?? documentType.NameDocumentType,
                            DocumentTypeName = documentType.NameDocumentType, // Toujours le nom du type de document (ex: "Lettre de motivation")
                            UploadDate = submittedDocument?.DocumentDepositDate ?? DateTime.MinValue, // Date de dépôt si soumis, sinon valeur minimale
                            DocumentPath = submittedDocument?.DocumentPath, // Chemin du fichier si soumis
                            IsVerified = submittedDocument?.IsVerified ?? false // Statut de validation (true si soumis ET validé, false sinon)
                        };
                    }).ToList() // Convertit le résultat en une liste
            };

            return viewModel;
        }
        public async Task<IEnumerable<CandidatureStudentViewModel>> GetAllCandidaturesForOverviewAsync()
        {
            // 1. Récupérer toutes les candidatures avec leurs détails.
            // C'est une bonne base, assurez-vous que GetAllCandidaturesWithDetailsAsync() inclut bien :
            // - User (pour FirstName, LastName, Email, Phone, Address, BirthDate, StudentImage)
            // - Class (pour NameClass)
            // - CandidatureStatus (pour Label)
            // - Documents (pour DocumentId, DocumentName, DocumentPath, IsVerified, et leur DocumentType lié pour NameDocumentType)
            var allCandidatures = await _candidatureRepository.GetAllCandidaturesWithDetailsAsync();

            // 2. Récupérer le nombre total de types de documents requis, de manière asynchrone.
            // C'est crucial pour le calcul des pourcentages.
            // ASSUREZ-VOUS que GetAllAsync() de _documentTypeRepository retourne BIEN tous les types de documents
            // qu'un étudiant DOIT potentiellement fournir pour une candidature complète.
            // Par exemple, si vous avez 5 types de documents (CV, Lettre de motivation, Diplôme, Carte ID, Relevé de notes)
            // cette liste devrait contenir 5 éléments.
            var allDocumentTypes = await _documentTypeRepository.GetAllAsync();
            var totalRequiredDocumentTypesCount = allDocumentTypes.Count();

            // 3. Mapper les entités Candidature vers une liste de CandidatureStudentViewModel
            var viewModels = allCandidatures
                .Where(c => c.User != null && !c.User.IsDeleted) // Filtre correct
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

                        // --- IMPORTANT : Décommentez ces lignes pour utiliser les données réelles ---
                        //CandidatureStatus = c.CandidatureStatus?.Label, // Assurez-vous que CandidatureStatus est inclus dans GetAllCandidaturesWithDetailsAsync()
                        //StudentImage = c.User?.StudentImage ?? "/images/default_student.png", // Utilisez la valeur par défaut du ViewModel si l'image de l'utilisateur est null/vide
                                                                                              // -----------------------------------------------------------------------

                        // Initialisation des progressions à 0 avant le calcul détaillé
                        // C'est correct, car elles seront mises à jour juste après.
                        StudentValidationProgress = 0,
                        MnsValidationProgress = 0,

                        // Mapper les documents liés à cette candidature
                        RequiredDocuments = c.DocumentTypes?.Select(d => new DocumentViewModel
                        {
                            DocumentId = d.DocumentId,
                            DocumentName = d.DocumentName, // Nom du document uploadé (ex: "mon_cv.pdf")
                            DocumentTypeName = d.DocumentType?.NameDocumentType, // Nom du type de document requis (ex: "Curriculum Vitae")
                            DocumentPath = d.DocumentPath,
                            IsVerified = d.IsVerified
                        }).ToList() ?? new List<DocumentViewModel>()
                    };

                    // 4. Calcul des progressions pour CHAQUE ViewModel (candidature)
                    // Ce bloc est exécuté pour chaque `vm` après son initialisation.
                    if (totalRequiredDocumentTypesCount > 0)
                    {
                        // Progression étudiant : basées sur les documents uploadés
                        // `vm.RequiredDocuments` contient uniquement les documents que cette candidature a.
                        // Il faut s'assurer que `c.DocumentTypes` ramène les documents *liés* à cette candidature
                        // et non pas tous les types de documents.
                        var uploadedDocumentsCount = vm.RequiredDocuments.Count(d => !string.IsNullOrEmpty(d.DocumentPath));
                        vm.StudentValidationProgress = (int)(((double)uploadedDocumentsCount / totalRequiredDocumentTypesCount) * 100);

                        // Progression MNS : basées sur les documents vérifiés
                        var verifiedDocumentsCount = vm.RequiredDocuments.Count(d => d.IsVerified);
                        vm.MnsValidationProgress = (int)(((double)verifiedDocumentsCount / totalRequiredDocumentTypesCount) * 100);
                    }
                    else
                    {
                        // Si aucun type de document n'est défini comme requis, la progression est 0.
                        // (Ou 100 si vous considérez qu'il n'y a rien à faire) - 0 est plus sûr ici.
                        vm.StudentValidationProgress = 0;
                        vm.MnsValidationProgress = 0;
                    }

                    return vm; // Retourne le ViewModel complété pour cette candidature
                })
                .ToList(); // Convertit le résultat du Select en List<CandidatureStudentViewModel>

            return viewModels;
        }



        public async Task<bool> UploadDocumentAsync(int candidatureId, IFormFile document, string documentTypeName)
        {
            // 1. Récupérer la candidature et le type de document
            var candidature = await _candidatureRepository.GetCandidatureByIdWithDetailsAsync(candidatureId);
            if (candidature == null) return false;

            // Logique pour identifier le document à uploader.


            var documentType = await _context.DocumentTypes.FirstOrDefaultAsync(dt => dt.NameDocumentType == documentTypeName);

            if (documentType == null)
            {
                // Aucun document à mettre à jour, ou tous les documents requis ont déjà un chemin
                return false;
            }

            // Traiter le fichier (sauvegarde physique)

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
                    existingDocument.DocumentPath = "uploads/documents/" + uniqueFileName; // Chemin relatif pour la DB
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
                        DocumentPath = "/uploads/documents/" + uniqueFileName,
                        DocumentDepositDate = DateTime.Now,
                        IsVerified = false // Par défaut, un nouveau document est en attente de validation
                    };
                    _context.Documents.Add(newDocument);
                }
                // 4. Mettre à jour les barres de progression (voir section suivante)
                // Ceci sera fait après l'enregistrement des modifications.

                await _context.SaveChangesAsync();

                // Calculer et mettre à jour la progression de l'étudiant
                await UpdateStudentValidationProgress(candidatureId); // Nouvelle méthode à créer


                return true;
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'upload du document : {ex.Message}");
                // Gérer l'erreur, peut-être logguer ou renvoyer un statut spécifique
                return false;
            }
        }





        // Dans CandidatureService.cs
        private async Task UpdateStudentValidationProgress(int candidatureId)
        {
            var candidature = await _context.Candidatures
.Include(c => c.DocumentTypes)
.FirstOrDefaultAsync(c => c.CandidatureId == candidatureId);

            if (candidature == null)
            {
                return;
            }
            // Exemple simple: 100% si tous les documents requis sont téléchargés
            // Vous devrez définir quels documents sont "requis".
            // Pour cet exemple, je vais compter tous les documents uploadés.
            var totalRequiredDocumentsCount = await _context.DocumentTypes.CountAsync(); // Nombre total de types de documents attendus
            int uploadedDocumentsCount = candidature.DocumentTypes.Count(d => !string.IsNullOrEmpty(d.DocumentPath));

            candidature.StudentValidationProgress = (int)Math.Round((double)uploadedDocumentsCount / totalRequiredDocumentsCount * 100);

            await _context.SaveChangesAsync();
        }

        // Cette méthode sera appelée après Validation/Rejet par l'admin
        private async Task UpdateMnsValidationProgress(int candidatureId)
        {
            var candidature = await _context.Candidatures
                                            .Include(c => c.DocumentTypes)
                                            .FirstOrDefaultAsync(c => c.CandidatureId == candidatureId);

            if (candidature == null) return;

            var totalRequiredDocumentsCount = await _context.DocumentTypes.CountAsync();
            var validatedDocumentsCount = candidature.DocumentTypes.Count(d => d.IsVerified);

            candidature.MnsValidationProgress = (int)Math.Round((double)validatedDocumentsCount / totalRequiredDocumentsCount * 100);

            await _context.SaveChangesAsync();
        }




        // Dans CandidatureService.cs
        public async Task<int> ValidateDocumentAsync(int documentId)
        {
            var document = await _context.Documents.FindAsync(documentId);
            if (document == null) return 0;

            document.IsVerified = true; // Valider le document
            await _context.SaveChangesAsync();

            await UpdateMnsValidationProgress(document.CandidatureId); // Mettre à jour la progression MNS
            return document.CandidatureId;
        }

        public async Task<int> RejectDocumentAsync(int documentId)
        {
            var document = await _context.Documents.FindAsync(documentId);
            if (document == null) return 0;

            document.IsVerified = false; // Rejeter (ou marquer comme non validé)
                                         // Vous pourriez ajouter une colonne pour la raison du rejet
            await _context.SaveChangesAsync();

            await UpdateMnsValidationProgress(document.CandidatureId); // Mettre à jour la progression MNS
            return document.CandidatureId;
        }




        public async Task<bool> DeleteCandidatureAsync(int id)
        {
            var candidature = await _candidatureRepository.GetByIdAsync(id);
            if (candidature == null) return false;

            // Optionnel : supprimer les fichiers physiques associés aux documents de la candidature
            if (candidature.DocumentTypes != null)
            {
                foreach (var doc in candidature.DocumentTypes.Where(d => !string.IsNullOrEmpty(d.DocumentPath)))
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

        public async Task<CandidatureStudentViewModel> GetCandidatureDetailsByUserIdAsync(string userId)
        {
            var candidature = await _context.Candidatures
                .Include(c => c.User)
                .Include(c => c.Class)
                .Include(c => c.CandidatureStatuses)
                .Include(c => c.DocumentTypes)
                .ThenInclude(d => d.DocumentType)
                .FirstOrDefaultAsync(c => c.UserId == userId);


            if (candidature == null)
            {
                return null;
            }

            var viewModel = new CandidatureStudentViewModel
            {
                CandidatureId = candidature.CandidatureId,
                CandidatureStatus = candidature.CandidatureStatuses.Label, // Ou CandidatureStutus, unifier les noms

                FirstName = candidature.User?.FirstName ?? "N/A",
                LastName = candidature.User?.LastName ?? "N/A",
                Email = candidature.User?.Email ?? "N/A",
                Phone = candidature.User?.PhoneNumber ?? "N/A",
                Address = candidature.User?.Address ?? "N/A",
                BirthDate = candidature.User?.BirthDate,
                StudentImage = string.IsNullOrEmpty(candidature.User?.Photo)
                               ? "/images/profiles/default_student.png"
                               : "/images/profiles/" + candidature.User.Photo,

                ClassName = candidature.Class?.NameClass ?? "N/A",

                StudentValidationProgress = 50, // Calcul à implémenter
                MnsValidationProgress = 30,     // Calcul à implémenter

                RequiredDocuments = candidature.DocumentTypes?.Select(d => new DocumentViewModel
                {
                    DocumentId = d.DocumentId,
                    DocumentName = d.DocumentName,
                    DocumentTypeName = d.DocumentType?.NameDocumentType ?? "Non défini",
                    UploadDate = d.DocumentDepositDate,
                    DocumentPath = d.DocumentPath != null ? $"/uploads/documents/{d.DocumentPath}" : null, // **AJUSTEZ CE CHEMIN**
                    IsVerified = d.IsVerified
                }).ToList() ?? new List<DocumentViewModel>()
            };

            return viewModel;
        }

        public Task<bool> UploadDocumentAsync(int candidatureId, IFormFile document, object documentTypeName)
        {
            throw new NotImplementedException();
        }
    }
}
