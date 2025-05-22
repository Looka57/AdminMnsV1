using AdminMnsV1.Models.Documents;
using AdminMnsV1.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using AdminMnsV1.Services.Interfaces; // L'interface IDocumentService
using System.IO; // Pour Stream
using System.Threading.Tasks;

namespace AdminMnsV1.Services.Implementation
{
    public class DocumentService : IDocumentService
    {
        private readonly IDocumentRepository _documentRepository;// Si besoin de CandidatureService pour mettre à jour la progression
        private readonly ICandidatureRepository _candidatureRepository;  // Une dépendance pour stocker physiquement le fichier (par exemple, un service de stockage de                                                             fichiers)
        private readonly string _uploadFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "documents"); // Chemin où stocker les fichiers


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

        public async Task<Documents> GetDocumentByIdAsync(int id)
        {
            return await _documentRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Documents>> GetDocumentsForCandidatureAsync(int candidatureId)
        {
            return await _documentRepository.GetDocumentsByCandidatureIdAsync(candidatureId);
        }



        public async Task<bool> UpdateDocumentStatusAsync(int documentId, string newStatus)
        {
            var document = await _documentRepository.GetDocumentWithDetailsAsync(documentId);
            if (document == null)
            {
                return false;
            }

            document.documentStatut = newStatus;
            _documentRepository.Update(document);
            var saved = await _documentRepository.SaveChangesAsync();

            if (saved > 0 && document.Candidature != null)
            {
                var candidatureDocuments = await _documentRepository.GetDocumentsByCandidatureIdAsync(document.CandidatureId);
                int totalDocuments = candidatureDocuments.Count();
                int validatedDocuments = candidatureDocuments.Count(d => d.documentStatut == "Validé");


                // Recalculer la progression de la candidature après la mise à jour du statut d'un document
                // Cela nécessitera d'accéder aux documents de la candidature
                if (totalDocuments > 0)
                {
                    document.Candidature.Progress = (int)Math.Round((double)validatedDocuments / totalDocuments * 100);
                }
                else
                {
                    document.Candidature.Progress = 0;
                }

                if (validatedDocuments == totalDocuments && totalDocuments > 0)
                {
                    document.Candidature.CandidatureStatutId = (await _candidatureRepository.GetCandidatureStatusIdByName("Validé")).Value;
                }
                else if (candidatureDocuments.Any(d => d.documentStatut == "Refusé"))
                {
                    document.Candidature.CandidatureStatutId = (await _candidatureRepository.GetCandidatureStatusIdByName("Refusé")).Value;
                }
                else if (document.Candidature.CandidatureStatutId != (await _candidatureRepository.GetCandidatureStatusIdByName("En cours")).Value)
                {
                    // Si ce n'est pas "En cours", et qu'il n'y a pas de documents refusés, on peut le remettre en cours.
                    // Attention : ce statut dépendra de ta logique métier.
                    document.Candidature.CandidatureStatutId = (await _candidatureRepository.GetCandidatureStatusIdByName("En cours")).Value;
                }

                _candidatureRepository.Update(document.Candidature);
                await _candidatureRepository.SaveChangesAsync();
            }
            return saved > 0;
        }

        public async Task<bool> UploadDocumentAsync(int candidatureId, int documentTypeId, string studentId, Stream fileStream, string fileName)
        {

            //Vérifier si le document existe déjà ou créer un nouvel enregistrement
            var documentToUpdate = (await _documentRepository.FindAsync(d =>
            d.CandidatureId == candidatureId &&
            d.DocumentTypeId == documentTypeId &&
            d.StudentId == studentId))
            .FirstOrDefault();

            if (documentToUpdate == null)
            {
                // Si le document n'existe pas ou n'est pas le bon type pour cet étudiant/candidature
                // C'est ici que tu devras décider si un upload crée un nouveau document inattendu ou rejette.
                // Pour l'exemple, on suppose qu'il doit exister un document en statut "Demandé" ou "Refusé" pour être uploadé.
                return false;
            }

            // Si le document existe déjà, on met à jour son statut et son fichier
            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(fileName);
            var filePath = Path.Combine(_uploadFolderPath, uniqueFileName);

            using (var outputStream = new FileStream(filePath, FileMode.Create))
            {
                await fileStream.CopyToAsync(outputStream);
            }

            // Mettre à jour les propriétés du document dans la base de données
            documentToUpdate.DocumentPath = "/documents/" + uniqueFileName;
            documentToUpdate.documentDepositDate = DateTime.Now;
            documentToUpdate.documentStatut = "Déposé"; // Statut initial après dépôt par le candidat


            _documentRepository.Update(documentToUpdate);
                var saved = await _documentRepository.SaveChangesAsync();

            // Après le dépôt, recalcule la progression de la candidature et met à jour son statut si nécessaire
            if (saved > 0)
            {
                // Cette logique est la même que dans UpdateDocumentStatusAsync, tu pourrais la factoriser
                var candidature = await _candidatureRepository.GetByIdAsync(candidatureId);
                if (candidature != null)
                {
                    var candidatureDocuments = await _documentRepository.GetDocumentsByCandidatureIdAsync(candidatureId);
                    int totalDocuments = candidatureDocuments.Count();
                    int validatedDocuments = candidatureDocuments.Count(d => d.documentStatut == "Validé");

                    if (totalDocuments > 0)
                    {
                        candidature.Progress = (int)Math.Round((double)validatedDocuments / totalDocuments * 100);
                    }
                    else
                    {
                        candidature.Progress = 0;
                    }

                    // Mettre à jour le statut global de la candidature
                    // Cette partie doit être robuste et correspondre à ta logique métier exacte
                    if (candidatureDocuments.Any(d => d.documentStatut == "Refusé"))
                    {
                        candidature.CandidatureStatutId = (await _candidatureRepository.GetCandidatureStatusIdByName("Refusé")).Value;
                    }
                    else if (candidatureDocuments.All(d => d.documentStatut == "Validé"))
                    {
                        candidature.CandidatureStatutId = (await _candidatureRepository.GetCandidatureStatusIdByName("Validé")).Value;
                    }
                    else
                    {
                        candidature.CandidatureStatutId = (await _candidatureRepository.GetCandidatureStatusIdByName("En cours")).Value;
                    }

                    _candidatureRepository.Update(candidature);
                    await _candidatureRepository.SaveChangesAsync();
                }
            }

            return saved > 0;





        }
    }
}