using AdminMnsV1.Models.Documents;


namespace AdminMnsV1.Repositories.Interfaces
{
    public interface IDocumentRepository : IGenericRepository<Documents>
    {
        Task<IEnumerable<Documents>> GetDocumentsByCandidatureIdAsync(int candidatureId);
        Task<Documents> GetDocumentWithDetailsAsync(int documentId); // Pour inclure DocumentType, Candidature, User etc.
    }
    
}
