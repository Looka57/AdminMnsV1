using System.ComponentModel.DataAnnotations;

namespace AdminMnsV1.Models.DocumentTypes
{
    public class DocumentType
    {
        [Key]
        public int DocumentTypeId { get; set; }
        [Required]
        public string? NameDocumentType { get; set; }
        public virtual ICollection<Documents.Documents> Documents { get; set; } = new List<Documents.Documents>();
    }
}
