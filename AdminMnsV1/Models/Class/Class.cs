using System.ComponentModel.DataAnnotations;

namespace AdminMnsV1.Models.Class
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

