// AdminMnsV1.Data.Repositories/Interfaces/IUserRepository.cs
using AdminMnsV1.Models; // Assure-toi que c'est le bon chemin pour ton modèle User
using System.Threading.Tasks;
using System.Collections.Generic;
using AdminMnsV1.Repositories.Interfaces;
using AdminMnsV1.Models; // Pour IEnumerable

namespace AdminMnsV1.Data.Repositories.Interfaces // <-- TRÈS IMPORTANT : VÉRIFIEZ CE NAMESPACE
{
    public interface IUserRepository : IGenericRepository<User> // Hérite de IGenericRepository
    {
        // Tu peux ajouter ici des méthodes spécifiques aux utilisateurs si besoin,
        // par exemple, GetUserByEmailAsync ou GetUserWithRolesAsync
        Task<User?> GetUserByEmailAsync(string email);
        Task<IEnumerable<User>> GetAllUsersWithDetailsAsync(); // Exemple si tu veux inclure des relations
        Task<bool> UpdateUserAsync(User user);
    }
}