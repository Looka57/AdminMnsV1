using System.Linq.Expressions;
using AdminMnsV1.Data;
using AdminMnsV1.Models;
using AdminMnsV1.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AdminMnsV1.Repositories.Implementation
{
    public class StatusRepository : IStatusRepository
    {
        private readonly ApplicationDbContext _context;

        public StatusRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DelayAbsStatus>> GetAllStatusesAsync()
        {
            return await _context.DelayAbsStatuses.ToListAsync();
        }

        public async Task<DelayAbsStatus> GetStatusByIdAsync(int id)
        {
            return await _context.DelayAbsStatuses.FindAsync(id);
        }


        public void Add(DelayAbsStatus entity)
        {
            throw new NotImplementedException();
        }

        public void AddRange(IEnumerable<DelayAbsStatus> entities)
        {
            throw new NotImplementedException();
        }

        public void Delete(DelayAbsStatus entity)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<DelayAbsStatus>> FindAsync(Expression<Func<DelayAbsStatus, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<DelayAbsStatus>> GetAllAbsenceAsync()
        {
            throw new NotImplementedException();
        }

  
        public Task<DelayAbsStatus?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }


        public void RemoveRange(IEnumerable<DelayAbsStatus> entities)
        {
            throw new NotImplementedException();
        }

        public Task<int> SaveChangesAsync()
        {
            throw new NotImplementedException();
        }

        public void Update(DelayAbsStatus entity)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<DelayAbsStatus>> GetAllAsync()
        {
            throw new NotImplementedException();
        }
    }
}
