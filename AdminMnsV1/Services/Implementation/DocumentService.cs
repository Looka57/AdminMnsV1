using AdminMnsV1.Models.Documents;
using AdminMnsV1.Repositories.Interfaces; // Utilise le chemin correct pour les interfaces de repositories (corrigé ici aussi)
using Microsoft.EntityFrameworkCore;
using AdminMnsV1.Application.Services.Interfaces; // Utilise le chemin correct pour les interfaces de services
using System.IO;
using System.Threading.Tasks;
using System.Linq; // Pour Count(), Any(), All()
using System;
using AdminMnsV1.Services.Interfaces; // Pour Math.Round
// J'ai vu AdminMnsV1.Services.Interfaces dans ton code, mais il n'est pas utilisé ici directement pour Math.Round.
// Je l'ai gardé pour éviter des breaking changes si tu en as besoin ailleurs.

namespace AdminMnsV1.Application.Services.Implementation
{
    public class DocumentService : IDocumentService
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly ICandidatureRepository _candidatureRepository;
        private readonly string _uploadFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "documents");

        public DocumentService(IDocumentRepository documentRepository, ICandidatureRepository candidatureRepository)
        {
            _documentRepository = documentRepository;
            _candidatureRepository = candidatureRepository;

            if (!Directory.Exists(_uploadFolderPath))
            {
                Directory.CreateDirectory(_uploadFolderPath);
            }
        }

        public async Task<IEnumerable<Documents>> GetAllDocumentsAsync()
        {
            return await _documentRepository.GetAllAsync();
        }

        public async Task<Documents?> GetDocumentByIdAsync(int id)
        {
            return await _documentRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Documents>> GetDocumentsForCandidatureAsync(int candidatureId)
        {
            return await _documentRepository.GetDocumentsByCandidatureIdAsync(candidatureId);
        }

        // NOUVELLE MÉTHODE POUR LA CRÉATION DE DOCUMENT AVEC UPLOAD DE FICHIER

        public async Task<(bool Success, string ErrorMessage)> CreateDocumentAsync(Documents document, IFormFile file, string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return (false, "L'ID de l'utilisateur n'est pas fourni.");
            }
            document.StudentId = userId;

            if (document.DocumentDepositDate == default(DateTime))
            {
                document.DocumentDepositDate = DateTime.Now;
            }


            if (file == null || file.Length == 0)
            {
                return (false, "Aucun fichier n'a été fourni.");
            }

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            var filePath = Path.Combine(_uploadFolderPath, uniqueFileName);

            try
            {
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                document.DocumentPath = "/document" + uniqueFileName; //Chemin relatif pour l'accès web
                _documentRepository.Add(document);
                await _documentRepository.SaveChangesAsync();

                if (document.CandidatureId > 0) // Vérifie si le document est lié à une candidature
                {
                    await UpdateCandidatureProgressAndStatus(document.CandidatureId);
                }

                return (true, null);

            }
            catch (Exception ex)
            {
                // En cas d'erreur de base de données, nettoyer le fichier uploadé si nécessaire
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                return (false, $"Erreur lors de la création du document : {ex.Message}");
            }
        }


        public async Task<bool> UpdateDocumentStatusAsync(int documentId, string newStatus)
        {
            var document = await _documentRepository.GetDocumentWithDetailsAsync(documentId);
            if (document == null)
            {
                return false;
            }

            var newStatusId = await _documentRepository.GetDocumentStatusIdByName(newStatus);
            if (newStatusId == null)
            {
                throw new InvalidOperationException($"Le statut de document '{newStatus}' n'existe pas dans la base de données.");
            }

            document.DocumentStatusId = (int)newStatusId; // CORRIGÉ : Cast direct après null check, enlève .Value
            _documentRepository.Update(document);
            var saved = await _documentRepository.SaveChangesAsync();

            if (saved > 0 && document.Candidature != null)
            {
                var candidatureDocuments = await _documentRepository.GetDocumentsByCandidatureIdAsync(document.CandidatureId);
                int totalDocuments = candidatureDocuments.Count();

                int validatedDocuments = candidatureDocuments.Count(d => d.DocumentStatus?.DocumentStatusName == "Validé");

                if (totalDocuments > 0)
                {
                    document.Candidature.Progress = (int)Math.Round((double)validatedDocuments / totalDocuments * 100);
                }
                else
                {
                    document.Candidature.Progress = 0;
                }

                // Récupère les IDs des statuts de candidature
                int? valideCandidatureStatusIdNullable = await _candidatureRepository.GetCandidatureStatusIdByName("Validé");
                int? refuseCandidatureStatusIdNullable = await _candidatureRepository.GetCandidatureStatusIdByName("Refusé");
                int? enCoursCandidatureStatusIdNullable = await _candidatureRepository.GetCandidatureStatusIdByName("En cours");

                // Utilise la propriété .Value de manière sécurisée ou le null-coalescing operator
                int valideCandidatureStatusId = valideCandidatureStatusIdNullable ?? throw new InvalidOperationException("Statut 'Validé' de candidature introuvable.");
                int refuseCandidatureStatusId = refuseCandidatureStatusIdNullable ?? throw new InvalidOperationException("Statut 'Refusé' de candidature introuvable.");
                int enCoursCandidatureStatusId = enCoursCandidatureStatusIdNullable ?? throw new InvalidOperationException("Statut 'En cours' de candidature introuvable.");


                if (validatedDocuments == totalDocuments && totalDocuments > 0)
                {
                    document.Candidature.CandidatureStatusId = valideCandidatureStatusId;
                }
                else if (candidatureDocuments.Any(d => d.DocumentStatus?.DocumentStatusName == "Refusé"))
                {
                    document.Candidature.CandidatureStatusId = refuseCandidatureStatusId;
                }
                else
                {
                    document.Candidature.CandidatureStatusId = enCoursCandidatureStatusId;
                }

                _candidatureRepository.Update(document.Candidature);
                await _candidatureRepository.SaveChangesAsync();
            }
            return saved > 0;
        }

        public async Task<bool> UploadDocumentAsync(int candidatureId, int documentTypeId, string studentId, Stream fileStream, string fileName)
        {
            var documentToUpdate = (await _documentRepository.FindAsync(d =>
                d.CandidatureId == candidatureId &&
                d.DocumentTypeId == documentTypeId &&
                d.StudentId == studentId)).FirstOrDefault();

            if (documentToUpdate == null)
            {
                return false;
            }

            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(fileName);
            var filePath = Path.Combine(_uploadFolderPath, uniqueFileName);

            using (var outputStream = new FileStream(filePath, FileMode.Create))
            {
                await fileStream.CopyToAsync(outputStream);
            }

            documentToUpdate.DocumentPath = "/documents/" + uniqueFileName;
            documentToUpdate.DocumentDepositDate = DateTime.Now;

            var deposeStatusId = await _documentRepository.GetDocumentStatusIdByName("Déposé");
            if (deposeStatusId == null)
            {
                throw new InvalidOperationException("Le statut de document 'Déposé' n'existe pas dans la base de données.");
            }
            documentToUpdate.DocumentStatusId = (int)deposeStatusId; // CORRIGÉ : Cast direct après null check, enlève .Value

            _documentRepository.Update(documentToUpdate);
            var saved = await _documentRepository.SaveChangesAsync();

            if (saved > 0)
            {
                var candidature = await _candidatureRepository.GetByIdAsync(candidatureId);
                if (candidature != null)
                {
                    var candidatureDocuments = await _documentRepository.GetDocumentsByCandidatureIdAsync(candidatureId);
                    int totalDocuments = candidatureDocuments.Count();
                    int validatedDocuments = candidatureDocuments.Count(d => d.DocumentStatus?.DocumentStatusName == "Validé");

                    if (totalDocuments > 0)
                    {
                        candidature.Progress = (int)Math.Round((double)validatedDocuments / totalDocuments * 100);
                    }
                    else
                    {
                        candidature.Progress = 0;
                    }

                    int? valideCandidatureStatusIdNullable = await _candidatureRepository.GetCandidatureStatusIdByName("Validé");
                    int? refuseCandidatureStatusIdNullable = await _candidatureRepository.GetCandidatureStatusIdByName("Refusé");
                    int? enCoursCandidatureStatusIdNullable = await _candidatureRepository.GetCandidatureStatusIdByName("En cours");

                    int valideCandidatureStatusId = valideCandidatureStatusIdNullable ?? throw new InvalidOperationException("Statut 'Validé' de candidature introuvable.");
                    int refuseCandidatureStatusId = refuseCandidatureStatusIdNullable ?? throw new InvalidOperationException("Statut 'Refusé' de candidature introuvable.");
                    int enCoursCandidatureStatusId = enCoursCandidatureStatusIdNullable ?? throw new InvalidOperationException("Statut 'En cours' de candidature introuvable.");


                    if (candidatureDocuments.Any(d => d.DocumentStatus?.DocumentStatusName == "Refusé"))
                    {
                        candidature.CandidatureStatusId = refuseCandidatureStatusId;
                    }
                    else if (candidatureDocuments.All(d => d.DocumentStatus?.DocumentStatusName == "Validé"))
                    {
                        candidature.CandidatureStatusId = valideCandidatureStatusId;
                    }
                    else
                    {
                        candidature.CandidatureStatusId = enCoursCandidatureStatusId;
                    }

                    _candidatureRepository.Update(candidature);
                    await _candidatureRepository.SaveChangesAsync();
                }
            }
            return saved > 0;
        }
        public async Task UpdateDocumentAsync(Documents document)
        {
            _documentRepository.Update(document);
            await _documentRepository.SaveChangesAsync();

            if (document.CandidatureId > 0) // Si 0 n'est pas un ID de candidature valide
            {
                await UpdateCandidatureProgressAndStatus(document.CandidatureId); // Accès direct à l'int
            }
        }

        public async Task DeleteDocumentAsync(int id)
        {
            var documentToDelete = await _documentRepository.GetByIdAsync(id); // Utilise GetByIdAsync du générique
            if (documentToDelete != null)
            {
                _documentRepository.Delete(documentToDelete); // Utilise Remove() du GenericRepository
                await _documentRepository.SaveChangesAsync(); // Sauvegarde les changements après la suppression
                // Si la suppression d'un document impacte la candidature, recalcule la progression
                if (documentToDelete.CandidatureId > 0)
                {
                    await UpdateCandidatureProgressAndStatus(documentToDelete.CandidatureId);
                }
            }
        }

        public async Task<bool> DocumentExistsAsync(int id)
        {
            return await _documentRepository.GetByIdAsync(id) != null; // Utilise GetByIdAsync du générique
        }

        // -------------------------------------------------

        // Méthode utilitaire privée pour mettre à jour la progression et le statut de la candidature
        private async Task UpdateCandidatureProgressAndStatus(int candidatureId)
        {
            var candidature = await _candidatureRepository.GetByIdAsync(candidatureId);
            if (candidature == null)
            {
                return;
            }

            var candidatureDocuments = await _documentRepository.GetDocumentsByCandidatureIdAsync(candidatureId);
            int totalDocuments = candidatureDocuments.Count();
            int validatedDocuments = candidatureDocuments.Count(d => d.DocumentStatus?.DocumentStatusName == "Validé");
            int refusedDocuments = candidatureDocuments.Count(d => d.DocumentStatus?.DocumentStatusName == "Refusé");

            if (totalDocuments > 0)
            {
                candidature.Progress = (int)Math.Round((double)validatedDocuments / totalDocuments * 100);
            }
            else
            {
                candidature.Progress = 0;
            }

            int valideCandidatureStatusId = (await _candidatureRepository.GetCandidatureStatusIdByName("Validé"))
                                             ?? throw new InvalidOperationException("Statut 'Validé' de candidature introuvable.");
            int refuseCandidatureStatusId = (await _candidatureRepository.GetCandidatureStatusIdByName("Refusé"))
                                             ?? throw new InvalidOperationException("Statut 'Refusé' de candidature introuvable.");
            int enCoursCandidatureStatusId = (await _candidatureRepository.GetCandidatureStatusIdByName("En cours"))
                                              ?? throw new InvalidOperationException("Statut 'En cours' de candidature introuvable.");

            if (refusedDocuments > 0)
            {
                candidature.CandidatureStatusId = refuseCandidatureStatusId;
            }
            else if (validatedDocuments == totalDocuments && totalDocuments > 0)
            {
                candidature.CandidatureStatusId = valideCandidatureStatusId;
            }
            else
            {
                candidature.CandidatureStatusId = enCoursCandidatureStatusId;
            }

            _candidatureRepository.Update(candidature);
            await _candidatureRepository.SaveChangesAsync();
        }



    }
}
