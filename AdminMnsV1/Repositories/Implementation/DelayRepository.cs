using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using AdminMnsV1.Data;
using AdminMnsV1.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AdminMnsV1.Repositories.Implementation
{
    public class DelayRepository : IDelayRepository
    {
        private readonly ApplicationDbContext _context;
        public DelayRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Delay>> GetAllDelaysAsync()
        {
            return await _context.Delays
                .Include(d => d.User)
                .Include(d => d.ReasonDelay)
                .Include(d => d.Status)
                .ToListAsync();
        }

        public async Task<Delay> GetDelayByIdAsync(int id)
        {
            return await _context.Delays
                .Include(d => d.User)
                .Include(d => d.ReasonDelay)
                .Include(d => d.Status)
                .FirstOrDefaultAsync(d => d.DelayId == id);
        }

        public async Task AddDelayAsync(Delay delay)
        {
            _context.Delays.Add(delay);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateDelayAsync(Delay delay)
        {
            _context.Entry(delay).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteDelayAsync(int id)
        {
            var delay = await _context.Delays.FindAsync(id);
            if (delay != null)
            {
                _context.Delays.Remove(delay);
                await _context.SaveChangesAsync();
            }
        }


        public async Task<IEnumerable<Delay>> GetPendingDelaysAsync()
        {
            return await _context.Delays
                .Where(d => d.Status.Label == "En attente")
                .Include(d => d.User)
                .Include(d => d.ReasonDelay)
                .Include(d => d.Status)
                .ToListAsync();
        }



































        public void Add(Delay entity)
        {
            throw new NotImplementedException();
        }

        public void AddRange(IEnumerable<Delay> entities)
        {
            throw new NotImplementedException();
        }

        public void Delete(Delay entity)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Delay>> FindAsync(Expression<Func<Delay, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Delay>> GetAllAbsenceAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Delay?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public void RemoveRange(IEnumerable<Delay> entities)
        {
            throw new NotImplementedException();
        }

        public Task<int> SaveChangesAsync()
        {
            throw new NotImplementedException();
        }

        public void Update(Delay entity)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Delay>> GetAllAsync()
        {
            throw new NotImplementedException();
        }
    }
}
