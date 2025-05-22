using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AdminMnsV1.Models.DocumentTypes;

namespace AdminMnsV1.Models.Documents
{
    public class Documents
    {
        [Key]
        public int DocumentId { get; set; }

        [Required]
        public string documentName { get; set; }

        [Required]
        public DateTime documentDepositDate { get; set; }

        [Required]
        public string documentStatut { get; set; }

        [Required]
        public string documentDateStatutValidate { get; set; }

        [Required]
        [StringLength(500)] // Adapte la longueur si les chemins sont très longs
        public string DocumentPath { get; set; } // Chemin physique ou URL du fichier

        //---------------Clé etranger vers DocumentType--------------
        public int DocumentTypeId { get; set; } // Clé étrangère vers l'ID de DocumentType (User.Id)
        [ForeignKey("DocumentTypeId")]
        public DocumentType DocumentType { get; set; }


        //---------------Clé etranger vers Student--------------
        public string StudentId { get; set; } // Clé étrangère vers l'ID du stagiaire (User.Id)
        [ForeignKey("StudentId")]
        public virtual User StudentUser { get; set; } // Propriété de navigation vers l'utilisateur étudiant


        //---------------- Pour la validation par l'administrateur------------
        public string? AdminId { get; set; } // Clé étrangère vers l'ID de l'administrateur (User.Id)
        [ForeignKey("AdminId")]
        public User? Admin { get; set; } // Navigation property vers l'admin qui a validé
        public DateTime? ValidationDate { get; set; } // Date de validation

        // Ajoute la clé étrangère vers Candidature
        public int CandidatureId { get; set; } // Ou int? si CandidatureId est int
        [ForeignKey("CandidatureId")]
        public virtual Candidature.Candidature? Candidature { get; set; }
    }
}
