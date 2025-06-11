using System.ComponentModel.DataAnnotations;

namespace AdminMnsV1.Models
{
    public class DelayAbsStatus
    {
        [Key]
        public int DelayAbsStatusId { get; set; } 

        [Required]
        [StringLength(50)]
        public string Label { get; set; }
    }
}

