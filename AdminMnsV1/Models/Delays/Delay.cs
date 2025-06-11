using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AdminMnsV1.Models;
using AdminMnsV1.Models.Delays;
using AdminMnsV1.Models.Students;

public class Delay
{
    [Key]
    public int DelayId { get; set; } 

    [Required]
    public DateTime ArrivalTime { get; set; } 

    [StringLength(150)]
    public string JustificationStatus { get; set; }

    public DateTime? DateValidated { get; set; }

    // Clés étrangères
    public string AdministratorId { get; set; } 
    public string StudentId { get; set; } 
    public string User { get; set; }
    public int StatusId { get; set; }
    public int ReasonDelayId { get; set; } 



    // Propriétés de navigation
    [ForeignKey("AdministratorId")]
    public Administrator? Administrator { get; set; }
    [ForeignKey("ReasonDelayId")]
    public ReasonDelay? ReasonDelay { get; set; }
    [ForeignKey("StudentId")]
    public Student? Student { get; set; }
    [ForeignKey("StatusId")]
    public DelayAbsStatus? Status { get; set; }
}
