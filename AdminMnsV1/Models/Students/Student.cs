using Microsoft.AspNetCore.Http.HttpResults;

namespace AdminMnsV1.Models.Students
{
   public class Student : User //Ceci déclare une classe publique nommée Student. Le : User indique que la classe Student hérite des propriétés et des méthodes de User
    {
        public string? Nationality { get; set; }
        public string? SocialSecurityNumber { get; set; }
        public string? FranceTravailNumber { get; set; }
        public string? Status { get; set; } // Peut contenir "Candidat" ou "Stagiaire"
        public string? Photo { get; set; }

        //Le ? après string signifie que cette propriété peut accepter la valeur null
    }
}
