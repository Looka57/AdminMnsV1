using System.Linq.Expressions;
using AdminMnsV1.Data;
using AdminMnsV1.Models.Delays;
using AdminMnsV1.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AdminMnsV1.Repositories.Implementation
{
    public class ReasonDelayRepository : IReasonDelayRepository

    {

        private readonly ApplicationDbContext _context;

        public ReasonDelayRepository(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<IEnumerable<ReasonDelay>> GetAllReasonDelaysAsync()
        {
            return await _context.ReasonDelays.ToListAsync();
        }

        public async Task<ReasonDelay> GetReasonDelayByIdAsync(int id)
        {
            return await _context.ReasonDelays.FindAsync(id);
        }












        public void Add(ReasonDelay entity)
        {
            throw new NotImplementedException();
        }

        public void AddRange(IEnumerable<ReasonDelay> entities)
        {
            throw new NotImplementedException();
        }

        public void Delete(ReasonDelay entity)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ReasonDelay>> FindAsync(Expression<Func<ReasonDelay, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ReasonDelay>> GetAllAbsenceAsync()
        {
            throw new NotImplementedException();
        }

    
        public Task<ReasonDelay?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

    

        public void RemoveRange(IEnumerable<ReasonDelay> entities)
        {
            throw new NotImplementedException();
        }

        public Task<int> SaveChangesAsync()
        {
            throw new NotImplementedException();
        }

        public void Update(ReasonDelay entity)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ReasonDelay>> GetAllAsync()
        {
            throw new NotImplementedException();
        }
    }
}
