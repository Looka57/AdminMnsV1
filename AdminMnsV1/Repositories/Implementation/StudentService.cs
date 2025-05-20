// Repositories/StudentRepository.cs
using AdminMnsV1.Data;
using AdminMnsV1.Models;
using AdminMnsV1.Models.Students;
using AdminMnsV1.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace AdminMnsV1.Repositories.Implementation
{
    public class StudentRepository : IStudentRepository
    {
        private readonly ApplicationDbContext _context;

        public StudentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Student>> GetAllStudentsWithDetailsAsync()
        {
            return await _context.Set<Student>()
                                 .Where(s => (s.Status == "Stagiaire" || s.Status == "Candidat") && !s.IsDeleted)
                                 .Include(s => s.Attends)
                                     .ThenInclude(a => a.Class)
                                 .ToListAsync();
        }

        public async Task<Student?> GetStudentByIdAsync(string id)
        {
            // Inclure les Attends si tu as besoin de l'info de classe pour un seul étudiant
            return await _context.Set<Student>()
                                 .Include(s => s.Attends)
                                     .ThenInclude(a => a.Class)
                                 .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);
        }

        public async Task AddAttendEntryAsync(Attend attendEntry)
        {
            await _context.Attends.AddAsync(attendEntry);
        }

        public async Task RemoveAttendEntriesAsync(string studentId)
        {
            var existingAttends = await _context.Attends
                                                .Where(a => a.StudentId == studentId)
                                                .ToListAsync();
            if (existingAttends.Any())
            {
                _context.Attends.RemoveRange(existingAttends);
            }
        }

        public async Task<bool> HasAttendEntriesAsync(string studentId)
        {
            return await _context.Attends.AnyAsync(a => a.StudentId == studentId);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}

//**Explication :**

//  *Il utilise `_context.Set<Student>()` car `Student` hérite de `User`.
//  * Les méthodes incluent le chargement (`Include`, `ThenInclude`) des relations (`Attends`, `Class`) car c'est nécessaire pour tes `ViewModels`.
//  * `SaveChangesAsync()` est isolé ici, mais le service décidera quand l'appeler.