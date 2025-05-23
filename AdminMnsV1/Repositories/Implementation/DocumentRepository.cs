using AdminMnsV1.Models.Documents;
using AdminMnsV1.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using AdminMnsV1.Data; // Assure-toi que ce using est correct selon ta structure


namespace AdminMnsV1.Repositories.Implementation
{
    public class DocumentRepository : GenericRepository<Documents>, IDocumentRepository
    {
            public DocumentRepository(ApplicationDbContext context) : base(context)
            {
                // Le constructeur de base (GenericRepository) initialise _context et _dbSet<Document>
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

        public async Task<int?> GetDocumentStatusIdByName(string statusName)
        {
            // Assure-toi que ton ApplicationDbContext a un DbSet<DocumentStatus>
            var status = await _context.DocumentStatuses.FirstOrDefaultAsync(s => s.DocumentStatusName == statusName);
            return status?.DocumentStatusId;
        }
    }
    }
