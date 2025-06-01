// AdminMnsV1.Data.Repositories/Interfaces/IDocumentTypeRepository.cs
using AdminMnsV1.Models.DocumentTypes; // Assure-toi que c'est le bon chemin pour ton modèle DocumentType
using AdminMnsV1.Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdminMnsV1.Data.Repositories.Interfaces // <-- TRÈS IMPORTANT : VÉRIFIEZ CE NAMESPACE
{
    public interface IDocumentTypeRepository : IGenericRepository<DocumentType> // Hérite de IGenericRepository
    {
       

    }
}