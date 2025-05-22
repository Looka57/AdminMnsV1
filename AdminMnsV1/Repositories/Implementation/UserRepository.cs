// AdminMnsV1.Data.Repositories/Implementation/UserRepository.cs
using AdminMnsV1.Data; // Pour ApplicationDbContext
using AdminMnsV1.Data.Repositories.Interfaces; // Pour IUserRepository
using AdminMnsV1.Models;
using AdminMnsV1.Repositories.Implementation;
using Microsoft.EntityFrameworkCore; // Pour .Include()
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminMnsV1.Data.Repositories.Implementation // <-- TRÈS IMPORTANT : VÉRIFIEZ CE NAMESPACE
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<IEnumerable<User>> GetAllUsersWithDetailsAsync()
        {
            // Exemple : si ton User a des relations que tu veux charger (ex: Roles, Candidatures, etc.)
            return await _dbSet
                // .Include(u => u.Roles) // Décommente si tu as des rôles liés à l'utilisateur
                // .Include(u => u.Candidatures) // Décommente si tu as des candidatures liées à l'utilisateur
                .ToListAsync();
        }
    }
}