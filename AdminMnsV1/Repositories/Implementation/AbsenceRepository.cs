using System.Linq.Expressions;
using AdminMnsV1.Data;
using AdminMnsV1.Models.Abscences;
using AdminMnsV1.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AdminMnsV1.Repositories.Implementation
{
    public class AbsenceRepository : IAbsenceRepository
    {
        private readonly ApplicationDbContext _context;

        public AbsenceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Absence>> GetAllAbsenceAsync()
        {
            return await _context.Absences
                .Include(a => a.User)
                .Include(a => a.ReasonAbsence)
                .Include(a => a.Status)
                .ToListAsync();
        }

        public async Task<Absence?> GetAbsenceByIdAsync(int id)
        {
            return await _context.Absences
                .Include(a => a.Student)
                .Include(a => a.ReasonAbsence)
                .Include(a => a.Status)
                .FirstOrDefaultAsync(a => a.AbsenceId == id);
        }


        public async Task AddAbsenceAsync(Absence absence)
        {
            _context.Absences.Add(absence);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAbsenceAsync(Absence absence)
        {
            _context.Entry(absence).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAbsenceAsync(int id)
        {
            var absence = await _context.Absences.FindAsync(id);
            if (absence != null)
            {
                _context.Absences.Remove(absence);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Absence>> GetPendingAbsencesAsync()
        {
            return await _context.Absences
               .Where(a => a.Status.Label == "En attente")
               .Include(a => a.User)
               .Include(a => a.ReasonAbsence)
               .Include(a => a.Status)
               .ToListAsync();
        }









        public void Add(Absence entity)
        {
            throw new NotImplementedException();
        }

        public void AddRange(IEnumerable<Absence> entities)
        {
            throw new NotImplementedException();
        }

        public void Delete(Absence entity)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Absence>> FindAsync(Expression<Func<Absence, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Absence>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Absence?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

     

        public void RemoveRange(IEnumerable<Absence> entities)
        {
            throw new NotImplementedException();
        }

        public Task<int> SaveChangesAsync()
        {
            throw new NotImplementedException();
        }

        public void Update(Absence entity)
        {
            throw new NotImplementedException();
        }
    }
}
