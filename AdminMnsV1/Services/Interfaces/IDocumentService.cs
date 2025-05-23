using AdminMnsV1.Models.Documents;


namespace AdminMnsV1.Services.Interfaces
{
    public interface IDocumentService
    {
        Task<IEnumerable<Documents>> GetAllDocumentsAsync();
        Task<Documents> GetDocumentByIdAsync(int id);

        // NOUVELLE MÉTHODE POUR LA CRÉATION VIA LE FORMULAIRE
        Task<(bool Success, string ErrorMessage)> CreateDocumentAsync(Documents document, IFormFile file, string userId);
        Task UpdateDocumentAsync(Documents document);
        Task DeleteDocumentAsync(int id);
        Task<bool> DocumentExistsAsync(int id);
        Task<IEnumerable<Documents>> GetDocumentsForCandidatureAsync(int candidatureId);
        Task<bool> UpdateDocumentStatusAsync(int documentId, string newStatus);
        Task<bool>UploadDocumentAsync(int candidatureId, int documentTypeId, string studentId, Stream fileStream, string  fileName);

        // Ajout des méthodes spécifiques du repository qui peuvent être utiles au service
        //Task<IEnumerable<Documents>> GetDocumentsByCandidatureIdAsync(int candidatureId);
        //Task<int?> GetDocumentStatusIdByName(string statusName);

      
    }
}
