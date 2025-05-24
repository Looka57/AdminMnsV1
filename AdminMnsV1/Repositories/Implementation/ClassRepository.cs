using AdminMnsV1.Data;
using AdminMnsV1.Models.Classes;
using AdminMnsV1.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic; // Assurez-vous que c'est le seul using pour les collections
using System.Linq;
using System.Threading.Tasks;
// REMOVE: using System.Collections; // Non nécessaire car nous utilisons IEnumerable<T>
// REMOVE: using System; // Non nécessaire si vous n'utilisez pas de NotImplementedException ici

namespace AdminMnsV1.Repositories.Implementation
{
    // ClassRepository hérite de notre GenericRepository et implémente IClassRepository
    public class ClassRepository : GenericRepository<SchoolClass>, IClassRepository
    {
        // REMOVE: private readonly ApplicationDbContext _context;
        // La classe GenericRepository (base) a déjà un champ _context que vous pouvez utiliser.
        // N'initialisez pas un nouveau _context ici, cela est redondant et source de confusion.

        public ClassRepository(ApplicationDbContext context) : base(context)
        {
            // Le constructeur de la classe de base (GenericRepository) gère déjà l'initialisation de _context.
            // REMOVE: _context = context;
        }

        // --- Implémentation des méthodes SPÉCIFIQUES à IClassRepository ---

        // Implémentation correcte de GetAllClassesAsync pour correspondre à l'interface
        public async Task<IEnumerable<SchoolClass>> GetAllClassesAsync()
        {
            // Utilisez 'base._context' pour accéder au contexte hérité si nécessaire,
            // ou simplement '_context' s'il est accessible dans cette portée (ce qui est le cas ici car il est protected)
            return await _context.SchoolClass.OrderBy(c => c.NameClass).ToListAsync();
        }

        // Implémentation de la méthode spécifique GetClassesWithStudentCountsAsync
        public async Task<IEnumerable<object>> GetClassesWithStudentCountsAsync()
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
                .OrderBy(g => g.Class.NameClass)
                .ToListAsync<object>();
        }

        // --- Méthodes héritées de GenericRepository<SchoolClass> ---
        // Toutes les méthodes comme Add, Update, Remove, GetByIdAsync, GetAllAsync (sans Classes), FindAsync, SaveChangesAsync
        // sont déjà IMPLÉMENTÉES dans GenericRepository<SchoolClass> et sont automatiquement héritées.
        // VOUS DEVEZ SUPPRIMER toutes les implémentations que vous aviez avec "throw new NotImplementedException()"
        // pour ces méthodes car elles sont en double ou en conflit avec l'héritage.

        // À SUPPRIMER :
        // public Task AddAsync(SchoolClass entity) { throw new NotImplementedException(); }
        // public Task DeleteAsync(int id) { throw new NotImplementedException(); }
        // public Task<IEnumerable<SchoolClass>> GetAllAsync() { throw new NotImplementedException(); } // Conflit avec le GetAllAsync du GenericRepository
        // public Task<SchoolClass?> GetByIdAsync(int id) { throw new NotImplementedException(); }
        // public Task SaveChangesAsync() { throw new NotImplementedException(); }
        // public Task UpdateAsync(SchoolClass entity) { throw new NotImplementedException(); }
        // ... et les lignes commentées en bas qui étaient des implémentations explicites en double.

        // Gardez cette méthode si elle est unique et non couverte par GenericRepository.
        public async Task<SchoolClass?> GetClassByIdAsync(int id)
        {
            return await _context.SchoolClass.FindAsync(id);
        }
    }
}