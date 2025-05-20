// Repositories/ClassRepository.cs
using AdminMnsV1.Data;
using AdminMnsV1.Models.Classes;
using AdminMnsV1.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminMnsV1.Repositories.Implementation
{
    public class ClassRepository : IClassRepository
    {
        private readonly ApplicationDbContext _context;

        public ClassRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<SchoolClass>> GetAllClassesAsync()
        {
            return await _context.SchoolClass.OrderBy(c => c.NameClass).ToListAsync();
        }

        public async Task<SchoolClass?> GetClassByIdAsync(int id)
        {
            return await _context.SchoolClass.FindAsync(id);
        }

        // Implémentation de la nouvelle méthode
        public async Task<List<object>> GetClassesWithStudentCountsAsync()
        {
            return await _context.Attends
                .Include(a => a.Class)
                .Where(a => a.Class != null && a.Student.Status == "Stagiaire" && !a.Student.IsDeleted)
                .GroupBy(a => a.Class)
                .Select(g => new
                {
                    Class = g.Key,
                    StudentCount = g.Count()
                })
                .OrderBy(g => g.Class.NameClass) // Ajouté pour un tri cohérent
                .ToListAsync<object>(); // Cast en List<object> car le type anonyme n'est pas public
        }
    }
}