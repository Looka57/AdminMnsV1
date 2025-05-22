// AdminMnsV1.Application.Services/Interfaces/IDocumentTypeService.cs
using AdminMnsV1.Models.Documents;
using AdminMnsV1.Models.DocumentTypes; // Assure-toi que c'est le bon chemin pour ton modèle DocumentType

namespace AdminMnsV1.Application.Services.Interfaces // Ou AdminMnsV1.Services.Interfaces
{
    public interface IDocumentTypeService
    {
        Task<IEnumerable<DocumentType>> GetAllDocumentTypesAsync();
        Task<DocumentType?> GetDocumentTypeByIdAsync(int id);
        Task<DocumentType?> GetDocumentTypeByNameAsync(string name); // Utile pour la logique de progression
        Task<bool> AddDocumentTypeAsync(DocumentType documentType); // Si tu as besoin d'ajouter des types de documents
        Task<bool> UpdateDocumentTypeAsync(DocumentType documentType);
        Task<bool> DeleteDocumentTypeAsync(int id);
    }
}