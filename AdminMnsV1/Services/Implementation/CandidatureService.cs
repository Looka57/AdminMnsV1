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
using AdminMnsV1.Data.Repositories.Interfaces; // Pour les includes

namespace AdminMnsV1.Application.Services.Implementation // <-- TRÈS IMPORTANT : CORRESPOND AU USING DANS PROGRAM.CS
{
    public class CandidatureService : ICandidatureService
    {
        private readonly ICandidatureRepository _candidatureRepository;
        private readonly IUserRepository _userRepository; // Pour créer l'utilisateur si nécessaire
        private readonly IDocumentRepository _documentRepository; // Pour créer les documents initiaux
        private readonly IDocumentTypeRepository _documentTypeRepository; // Pour obtenir les types de documents
        private readonly IGenericRepository<CandidatureStatus> _candidatureStatusRepository; // Pour obtenir les statuts

        public CandidatureService(
            ICandidatureRepository candidatureRepository,
            IUserRepository userRepository,
            IDocumentRepository documentRepository,
            IDocumentTypeRepository documentTypeRepository,
            IGenericRepository<CandidatureStatus> candidatureStatusRepository) // Injecte le repository de statuts
        {
            _candidatureRepository = candidatureRepository;
            _userRepository = userRepository;
            _documentRepository = documentRepository;
            _documentTypeRepository = documentTypeRepository;
            _candidatureStatusRepository = candidatureStatusRepository;
        }

        public async Task<bool> CreateCandidatureAsync(CreateCandidatureViewModel model)
        {
            // 1. Vérifier si l'utilisateur existe déjà ou le créer
            var user = (await _userRepository.FindAsync(u => u.Email == model.Email)).FirstOrDefault();
            if (user == null)
            {
                user = new User
                {
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    // Ajoute d'autres propriétés de l'utilisateur si nécessaire (ex: rôle, password si création complète)
                    // Pour l'instant, nous ne gérons pas le mot de passe ici.
                };
                _userRepository.Add(user);
                await _userRepository.SaveChangesAsync(); // Sauvegarde l'utilisateur pour obtenir son ID
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
                    var demandedDocumentStatusId = await _documentRepository.GetDocumentStatusIdByName("Demandé");
                    if (demandedDocumentStatusId == null)
                    {
                        throw new InvalidOperationException("Le statut de document 'Demandé' n'existe pas dans la base de données.");
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
                                DocumentName = "Document Initial", // Ajout d'un nom de document par défaut
                                DocumentDepositDate = DateTime.Now // Ajout d'une date de dépôt par défaut
                            };
                            _documentRepository.Add(document);
                        }
                    }
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