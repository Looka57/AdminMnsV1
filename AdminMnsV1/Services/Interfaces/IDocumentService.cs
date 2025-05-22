using AdminMnsV1.Models.Documents;


namespace AdminMnsV1.Services.Interfaces
{
    public interface IDocumentService
    {
        Task<IEnumerable<Documents>> GetAllDocumentsAsync();
        Task<Documents> GetDocumentByIdAsync(int id);
        Task<IEnumerable<Documents>> GetDocumentsForCandidatureAsync(int candidatureId);
        Task<bool> UpdateDocumentStatusAsync(int documentId, string newStatus);
        Task<bool>UploadDocumentAsync(int candidatureId, int documentTypeId, string studentId, Stream fileStream, string  fileName);   
    }
}
