using AdminMnsV1.Models.Classes;
using AdminMnsV1.Models.Students;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdminMnsV1.Models
{
    public class Attend
    {
        // Clé étrangère vers la table Student
        [ForeignKey("Student")]
        public string StudentId { get; set; }
        public Student Student { get; set; }


        // Clé étrangère vers la table Class
        [ForeignKey("Class")]
        public int ClasseId { get; set; }
        public SchoolClass Class { get; set; }

    }
}
