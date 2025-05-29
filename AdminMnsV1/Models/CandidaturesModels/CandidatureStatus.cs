using System.ComponentModel.DataAnnotations;
using System.Collections.Generic; // N'oublie pas d'ajouter ce using !
// Si Candidature est dans un sous-namespace, tu devras peut-être l'importer ici aussi
using AdminMnsV1.Models.CandidaturesModels; // Exemple si Candidature est dans un dossier Candidature

namespace AdminMnsV1.Models
{
    public class CandidatureStatus
    {
        [Key]
        public int CandidatureStatusId { get; set; }

        [Required]
        [StringLength(50)]
        public string Label { get; set; }
        public ICollection<AdminMnsV1.Models.CandidaturesModels.Candidature> Candidatures { get; set; } = new List<AdminMnsV1.Models.CandidaturesModels.Candidature>();

    }
}