using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AdminMnsV1.Models.Classes;
using System;
using System.Collections.Generic; // Pour ICollection

namespace AdminMnsV1.Models.CandidaturesModels

{
    public class Candidature
    {
        [Key]
        public int CandidatureId { get; set; } // Clé primaire

        
        public DateTime? CandidatureCreationDate { get; set; }

        
        public DateTime? CandidatureValidationDate { get; set; }


        //Cle etranger vers Student qui postule (User.Id)
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }


        // Clé étrangère vers la classe (si la candidature est pour une classe spécifique)
        public int ClassId { get; set; }
        [ForeignKey("ClassId")]
        public SchoolClass? Class { get; set; }

        //Clé étrangère vers CandidatureStatus
        public int CandidatureStatusId { get; set; } 
        [ForeignKey("CandidatureStatusId")] // 
        public CandidatureStatus CandidatureStatus { get; set; }



        // Relation de navigation pour les documents liés à cette candidature
        public virtual ICollection<Documents.Documents>? DocumentTypes { get; set; }

        // Propriété de progression
        public int Progress { get; set; }
        public int StudentValidationProgress { get;  set; }
        public int MnsValidationProgress { get;  set; }
        //public string StudentId { get; set; }

    }
}
