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


        public CandidatureService(
            ICandidatureRepository candidatureRepository,
            IUserRepository userRepository,
            IDocumentRepository documentRepository,
            IDocumentTypeRepository documentTypeRepository,
            IGenericRepository<CandidatureStatus> candidatureStatusRepository,
            IGenericRepository<SchoolClass> classRepository,
            UserManager<User> userManager)
            
            

        {
            _candidatureRepository = candidatureRepository;
            _userRepository = userRepository;
            _documentRepository = documentRepository;
            _documentTypeRepository = documentTypeRepository;
            _candidatureStatusRepository = candidatureStatusRepository;
            _classRepository = classRepository;
            _userManager = userManager;

        }


        public async Task<CreateCandidatureViewModel>PrepareCreateCandidatureViewModelAsync()
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
            // 1. Vérifier si l'utilisateur existe déjà ou le créer
            var user = (await _userRepository.FindAsync(u => u.Email == model.Email)).FirstOrDefault();
            if (user == null)
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
                _userRepository.Add(user);
                await _userRepository.SaveChangesAsync(); // Sauvegarde l'utilisateur pour obtenir son ID
              
            }

            // Attribuer le rôle ASP.NET Identity "Student"
            // Ceci est fait qu'un nouvel utilisateur soit créé ou qu'un utilisateur existant soit trouvé
            // et n'ait pas encore le rôle "Student".
            if (!(await _userManager.IsInRoleAsync(user, "Student"))) // Vérifie si l'utilisateur n'a PAS déjà le rôle "Student"
            {
                var result = await _userManager.AddToRoleAsync(user, "Student"); // NOUVEAU : Attribution directe du rôle "Student"
                if (!result.Succeeded)
                {
                    // Gérer l'échec d'attribution de rôle (loguer l'erreur, lancer une exception, etc.)
                    // C'est crucial pour la sécurité et la fonctionnalité d'authentification.
                    throw new InvalidOperationException($"Échec d'attribution du rôle 'Student' à l'utilisateur : {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }

            // 2. Récupérer le statut "En cours"
            var enCoursStatus =( await _candidatureStatusRepository.FindAsync(s => s.Label == "En cours")).FirstOrDefault();
            if (enCoursStatus == null)
            {
                throw new InvalidOperationException("Le statut 'En cours' n'existe pas dans la base de données.");
            }



            // 3. Créer la candidature
            var candidature = new Candidature
            {
                UserId = user.Id, // L'ID de l'utilisateur créé ou trouvé
                ClassId = model.ClassId,
                CandidatureCreationDate = DateTime.Now,
                CandidatureStatusId = enCoursStatus.CandidatureStatusId, // Statut initial "En cours"

                Progress = 0 // Initialement 0%
            };
            _candidatureRepository.Add(candidature);
            var saved = await _candidatureRepository.SaveChangesAsync();



            if (saved > 0)
            {
                // 4. Créer les documents initialement requis avec le statut "Demandé"
                if (model.RequiredDocumentTypeIds != null && model.RequiredDocumentTypeIds.Any())
                {
                    // Récupère l'ID du statut "Demandé" pour les documents
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
                                DocumentStatusId = (int)demandedDocumentStatusId, // CORRIGÉ : Utilise l'ID du statut, enlève .Value
                                DocumentPath = "N/A", // Pas de fichier initialement
                                DocumentName = $"Document pour {documentType.NameDocumentType}", // UTILISE LE NOM DU TYPE DE DOCUMENT
                                DocumentDepositDate = DateTime.Now // Ajout d'une date de dépôt par défaut
                            };
                            _documentRepository.Add(document);
                        }
                    }

                    // STOCKER LE RÉSULTAT DE LA SAUVEGARDE DES DOCUMENTS ICI
                    var savedDocumentsCount = await _documentRepository.SaveChangesAsync(); // <-- NOUVELLE LIGNE
                    Console.WriteLine($"Tentative de sauvegarde de documents. Nombre de documents sauvegardés : {savedDocumentsCount}"); // POUR LE DÉBOGAGE
                    await _documentRepository.SaveChangesAsync();
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
            var status =(await _candidatureStatusRepository.FindAsync(s => s.Label == statusName)).FirstOrDefault();
            return status?.CandidatureStatusId;
        }

        public async Task<bool> UpdateCandidatureAsync(Candidature candidature)
        {
            _candidatureRepository.Update(candidature);
            return await _candidatureRepository.SaveChangesAsync() > 0;
        }
    }
}