using System.ComponentModel.DataAnnotations;

namespace AdminMnsV1.Models.Absences
{
    public class ReasonAbsence
    {
        [Key]
        public int ReasonAbsenceId { get; set; } 

        [Required]
        [StringLength(150)]
        public string ReasonWording { get; set; }
    }
}
