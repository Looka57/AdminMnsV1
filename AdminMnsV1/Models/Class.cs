using System.ComponentModel.DataAnnotations;
using AdminMnsV1.Models;

namespace AdminMnsV1.Models
{
    public class Class
    {
        [Key]
        public int ClasseId { get; set; }
        [Required]
        public string? NameClass { get; set; }
        public int AcademicYear { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
    }
}

