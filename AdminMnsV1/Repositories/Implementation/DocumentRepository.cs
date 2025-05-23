using AdminMnsV1.Models.Documents;
using AdminMnsV1.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using AdminMnsV1.Data;


namespace AdminMnsV1.Repositories.Implementation
{
    public class DocumentRepository : GenericRepository<Documents>, IDocumentRepository
    {
        public DocumentRepository(ApplicationDbContext context) : base(context)
        {
            // Le constructeur de base (GenericRepository) initialise _context et _dbSet<Document>
        }


        // Implémentation spécifique pour GetAllDocumentsAsync avec Includes
        public async Task<IEnumerable<Documents>> GetAllDocumentsAsync()
        {
            return await _dbSet
                .Include(d => d.DocumentType)
                .Include(d => d.DocumentStatus)
                .Include(d => d.StudentUser)
                .Include(d => d.Candidature)
                .Include(d => d.Admin)
                .ToListAsync();
        }

        // Implémentation spécifique pour GetDocumentStatusIdByName
        public async Task<int?> GetDocumentStatusIdByName(string statusName)
        {
            return await _context.DocumentStatuses 
                                 .Where(ds => ds.DocumentStatusName == statusName)
                                 .Select(ds => ds.DocumentStatusId)
                                 .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Documents>> GetDocumentsByCandidatureIdAsync(int candidatureId)
        {
            return await _dbSet
                .Where(d => d.CandidatureId == candidatureId)
                .Include(d => d.DocumentType) // Pour charger les infos sur le type de document
                .ToListAsync();
        }

        public async Task<Documents> GetDocumentWithDetailsAsync(int documentId)
        {
            return await _dbSet
             .Include(d => d.DocumentType)
             .Include(d => d.Candidature) // Si tu as besoin d'infos sur la candidature parente
             .Include(d => d.StudentUser) // Si tu as besoin d'infos sur l'étudiant propriétaire
             .FirstOrDefaultAsync(d => d.DocumentId == documentId);
        }

    }
}
