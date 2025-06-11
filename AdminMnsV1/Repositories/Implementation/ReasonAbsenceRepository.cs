using System.Linq.Expressions;
using AdminMnsV1.Data;
using AdminMnsV1.Models.Absences;
using AdminMnsV1.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AdminMnsV1.Repositories.Implementation
{
    public class ReasonAbsenceRepository : IReasonAbsenceRepository
    {

        private readonly ApplicationDbContext _context;

        public ReasonAbsenceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ReasonAbsence>> GetAllReasonAbsencesAsync()
        {
            return await _context.ReasonAbsences.ToListAsync();
        }

        public async Task<ReasonAbsence> GetReasonAbsenceByIdAsync(int id)
        {
            return await _context.ReasonAbsences.FindAsync(id);
        }
















        public void Add(ReasonAbsence entity)
        {
            throw new NotImplementedException();
        }

        public void AddRange(IEnumerable<ReasonAbsence> entities)
        {
            throw new NotImplementedException();
        }

        public void Delete(ReasonAbsence entity)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ReasonAbsence>> FindAsync(Expression<Func<ReasonAbsence, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ReasonAbsence>> GetAllAbsenceAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ReasonAbsence>> GetAllReasonsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ReasonAbsence?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ReasonAbsence>> GetPendingReasonsAsync()
        {
            throw new NotImplementedException();
        }

        public void RemoveRange(IEnumerable<ReasonAbsence> entities)
        {
            throw new NotImplementedException();
        }

        public Task<int> SaveChangesAsync()
        {
            throw new NotImplementedException();
        }

        public void Update(ReasonAbsence entity)
        {
            throw new NotImplementedException();
        }
    }
}
