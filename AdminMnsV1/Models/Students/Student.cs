using AdminMnsV1.Models.Classes;
using Microsoft.AspNetCore.Http.HttpResults;

namespace AdminMnsV1.Models.Students
{
   public class Student : User //Ceci déclare une classe publique nommée Student. Le : User indique que la classe Student hérite des propriétés et des méthodes de User
    {
       
        // Collection de la table de jointure (Attend)
        public virtual ICollection<Attend> Attends { get; set; } = new List<Attend>();

        // Collection des classes via la table de jointure Attend
        public virtual ICollection<SchoolClass> Classes { get; set; } = new List<SchoolClass>();

    }
}
