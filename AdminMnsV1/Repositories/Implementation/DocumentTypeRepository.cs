// AdminMnsV1.Data.Repositories/Implementation/DocumentTypeRepository.cs
using AdminMnsV1.Data; // Pour ApplicationDbContext
using AdminMnsV1.Data.Repositories.Interfaces; // Pour IDocumentTypeRepository
using AdminMnsV1.Models.DocumentTypes; // Pour le modèle DocumentType
using AdminMnsV1.Repositories.Implementation;
using Microsoft.EntityFrameworkCore; // Pour .Include() si nécessaire, ou AsNoTracking()
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminMnsV1.Data.Repositories.Implementation 
{
    public class DocumentTypeRepository : GenericRepository<DocumentType>, IDocumentTypeRepository
    {
        public DocumentTypeRepository(ApplicationDbContext context) : base(context)
        {
        }

        // Si tu as des méthodes spécifiques à implémenter de IDocumentTypeRepository, mets-les ici.
        // Par exemple, si tu as GetDocumentTypeByNameAsync dans l'interface, tu l'implémenterais ici :
        // public async Task<DocumentType?> GetDocumentTypeByNameAsync(string name)
        // {
        //     return await _dbSet.FirstOrDefaultAsync(dt => dt.DocumentTypeName == name);
        // }
    }
}