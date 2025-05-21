using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AdminMnsV1.Models.Classes;

namespace AdminMnsV1.Models.Candidature
{
    public class Candidature
    {
        [Key]
        public string CandidatureId { get; set; } // Clé primaire

        [Required]
        public DateTime CandidatureCreationDate { get; set; }

        [Required]
        public DateTime CandidatureValidationDate { get; set; }




        //Cle etranger vers Student qui postule (User.Id)
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }


        // Clé étrangère vers la classe (si la candidature est pour une classe spécifique)
        public int ClassId { get; set; }
        [ForeignKey("ClassId")]
        public SchoolClass? Class { get; set; }

        //Clé étrangère vers CandidatureStatus
        public int candidatureStatutId { get; set; }
        [ForeignKey("candidatureStatutId")]
        public CandidatureStatus CandidatureStatus { get; set; }



    }
}
