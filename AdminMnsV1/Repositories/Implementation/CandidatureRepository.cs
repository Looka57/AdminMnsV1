using AdminMnsV1.Data;
using AdminMnsV1.Models;
using AdminMnsV1.Models.CandidaturesModels;
using AdminMnsV1.Repositories.Implementation;
using AdminMnsV1.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace AdminMnsV1.Data.Repositories
{
    public class CandidatureRepository : GenericRepository<Candidature>, ICandidatureRepository
    {
        private readonly ApplicationDbContext _context;
        public CandidatureRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Candidature>> GetAllCandidaturesWithDetailsAsync()
        {
            return await _dbSet
                .Include(c => c.User)
                .Include(c => c.Class)
                .Include(c => c.CandidatureStatus)
                .ToListAsync();
        }


        public async Task<Candidature?> GetCandidatureByIdWithDetailsAsync(int id)
        {
            return await _context.Candidatures
                                 .Include(c => c.User)
                                 .Include(c => c.Class)
                                 .Include(c => c.DocumentTypes)
                                     .ThenInclude(d => d.DocumentType)
                                 .FirstOrDefaultAsync(c => c.CandidatureId == id);
        }

        // L'implémentation de la méthode GetCandidatureStatusIdByName
        public async Task<int?> GetCandidatureStatusIdByName(string statusName)
        {
            // Accès direct au DbSet des CandidatureStatus via le _context
            var status = await _context.CandidatureStatuses.FirstOrDefaultAsync(s => s.Label == statusName);
            return status?.CandidatureStatusId; // Retourne l'ID ou null si non trouvé
        }
    }
}