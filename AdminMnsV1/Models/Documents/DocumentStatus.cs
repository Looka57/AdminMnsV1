// AdminMnsV1.Models/Documents/DocumentStatus.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
// Pas besoin de AdminMnsV1.Models.DocumentTypes ici, DocumentStatus n'a pas besoin de ça

namespace AdminMnsV1.Models.Documents
{
    public class DocumentStatus
    {
        [Key]
        public int DocumentStatusId { get; set; }

        [Required]
        [StringLength(50)]
        public string DocumentStatusName { get; set; } // Ex: "Demandé", "Reçu", "Validé", "Refusé"

        // Si tu as une collection de Documents qui ont ce statut, ajoute-la ici:
        public virtual ICollection<Documents> Documents { get; set; } = new List<Documents>();
    }
}