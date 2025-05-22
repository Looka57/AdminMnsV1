using AdminMnsV1.Data;
using AdminMnsV1.Models.Candidature;
using AdminMnsV1.Repositories.Implementation;
using AdminMnsV1.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AdminMnsV1.Data.Repositories
{
    public class CandidatureRepository : GenericRepository<Candidature>, ICandidatureRepository
    {
        public CandidatureRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Candidature>> GetAllCandidaturesWithDetailsAsync()
        {
            return await _dbSet
                .Include(c => c.User)
                .Include(c => c.Class)
                .Include(c => c.CandidatureStatus)
                .ToListAsync();
        }

        public async Task<Candidature> GetCandidatureByIdWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(c => c.User)
                .Include(c => c.Class)
                .Include(c => c.CandidatureStatus)
                .Include(c => c.Documents)
                .ThenInclude(d => d.DocumentType)
                .FirstOrDefaultAsync(c => c.CandidatureId == id);
        }

        // L'implémentation de la méthode GetCandidatureStatusIdByName
        public async Task<int?> GetCandidatureStatusIdByName(string statusName)
        {
            // Accès direct au DbSet des CandidatureStatus via le _context
            // Assure-toi que ton ApplicationDbContext a un DbSet pour CandidatureStatus
            // Ex: public DbSet<CandidatureStatus> CandidatureStatuses { get; set; }
            var status = await _context.CandidatureStatuses.FirstOrDefaultAsync(s => s.Label == statusName);
            return status?.CandidatureStatusId; // Retourne l'ID ou null si non trouvé
        }
    }
}