using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AdminMnsV1.Models.Absences;
using AdminMnsV1.Models.Students;

namespace AdminMnsV1.Models.Abscences
{
    public class Absence
    {
        [Key] 
        public int AbsenceId { get; set; } 

        [Required]
        public DateTime StartDate { get; set; } 

        [Required]
        public DateTime EndDate { get; set; } 

        [StringLength(150)]
        public string JustificationStatus { get; set; } 

        public DateTime? DateValidated { get; set; } 



        // Clés étrangères
        public string AdministratorId { get; set; } 
        public string StudentId { get; set; } 

        public int ReasonAbsenceId { get; set; } 
        public int StatusId { get; set; } 



        // Propriétés de navigation pour Entity Framework Core (permettent de charger les objets liés)
        [ForeignKey("AdministratorId")]
        public Administrator? Administrator { get; set; } 
        [ForeignKey("StudentId")]
        public Student? Student { get; set; } 
        [ForeignKey("ReasonAbsenceId")]


        public ReasonAbsence? ReasonAbsence { get; set; }


        [ForeignKey("StatusId")]
        public DelayAbsStatus? Status { get; set; }
    }
}