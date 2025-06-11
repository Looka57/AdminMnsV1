using System.ComponentModel.DataAnnotations;

namespace AdminMnsV1.Models.Delays
{
    public class ReasonDelay
    {
        [Key]
        public int ReasonDelayId { get; set; } 

        [Required]
        [StringLength(50)]
        public string Label { get; set; } 
    }
}
