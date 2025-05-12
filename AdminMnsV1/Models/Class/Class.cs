using System.ComponentModel.DataAnnotations;
using AdminMnsV1.Models.Students;

namespace AdminMnsV1.Models.Classes
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


        public ICollection<Attend> Attends {get; set;}
        public ICollection<Student> Students { get; set; }
    }
}

