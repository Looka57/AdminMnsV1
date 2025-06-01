using System; // Pour DateTime
using AdminMnsV1.Models.ViewModels;

namespace AdminMnsV1.Models.ViewModels
{
    public class DocumentViewModel
    {
        public int DocumentId { get; set; }
        public string? DocumentName { get; set; } // Ajouté pour correspondre à votre mappage
        public string? DocumentTypeName { get; set; }
        public DateTime? DocumentDepositDate { get; set; }
        public string? DocumentStatut { get; set; } // Ajouté car vous l'avez mentionné
        public string? DocumentPath { get; set; }
        public int DocumentTypeId { get; set; } // ID du type de document
        public DateTime? UploadDate { get; set; } // Renommé de 'object' à 'DateTime?' pour être plus précis
        public bool IsVerified { get; set; }
    }
}