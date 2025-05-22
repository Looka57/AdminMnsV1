// AdminMnsV1.Application.Services/Implementation/DocumentTypeService.cs
using AdminMnsV1.Application.Services.Interfaces; // Ou AdminMnsV1.Services.Interfaces
using AdminMnsV1.Repositories.Interfaces; // Pour IGenericRepository<DocumentType>
using AdminMnsV1.Models.Documents; // Pour le modèle DocumentType
using AdminMnsV1.Models.DocumentTypes;
using AdminMnsV1.Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdminMnsV1.Application.Services.Implementation // Ou AdminMnsV1.Services.Implementation
{
    public class DocumentTypeService : IDocumentTypeService
    {
        private readonly IGenericRepository<DocumentType> _documentTypeRepository;

        public DocumentTypeService(IGenericRepository<DocumentType> documentTypeRepository)
        {
            _documentTypeRepository = documentTypeRepository;
        }

        public async Task<IEnumerable<DocumentType>> GetAllDocumentTypesAsync()
        {
            return await _documentTypeRepository.GetAllAsync();
        }

        public async Task<DocumentType?> GetDocumentTypeByIdAsync(int id)
        {
            return await _documentTypeRepository.GetByIdAsync(id);
        }

        public async Task<DocumentType?> GetDocumentTypeByNameAsync(string name)
        {
            // Note : Assure-toi que ton GenericRepository ou DocumentTypeRepository a une méthode FindAsync qui peut gérer des expressions
            // Ou utilise _documentTypeRepository.GetAllAsync() et filtre en mémoire si la liste est petite.
            // Pour une meilleure performance, il faudrait une méthode spécifique dans le repository si ce n'est pas déjà le cas.
            return (await _documentTypeRepository.FindAsync(dt => dt.NameDocumentType == name)).FirstOrDefault();
        }

        public async Task<bool> AddDocumentTypeAsync(DocumentType documentType)
        {
             _documentTypeRepository.Add(documentType);
            return await _documentTypeRepository.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateDocumentTypeAsync(DocumentType documentType)
        {
            _documentTypeRepository.Update(documentType);
            return await _documentTypeRepository.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteDocumentTypeAsync(int id)
        {
            var documentTypeToDelete = await _documentTypeRepository.GetByIdAsync(id);
            if (documentTypeToDelete == null) return false;

            _documentTypeRepository.Remove(documentTypeToDelete);
            return await _documentTypeRepository.SaveChangesAsync() > 0;
        }
    }
}