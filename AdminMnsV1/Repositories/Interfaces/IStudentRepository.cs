// Repositories/Interfaces/IStudentRepository.cs
using AdminMnsV1.Models;
using AdminMnsV1.Models.Students;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdminMnsV1.Repositories.Interfaces
{
    public interface IStudentRepository
    {
        Task<List<Student>> GetAllStudentsWithDetailsAsync();
        Task<Student> GetStudentByIdAsync(string id); 
        Task AddAttendEntryAsync(Attend attendEntry);
        Task RemoveAttendEntriesAsync(string studentId);
        Task SaveChangesAsync(); // Pour les opérations groupées
        Task<bool> HasAttendEntriesAsync(string studentId); // Pour vérifier si des inscriptions existent
    }
}