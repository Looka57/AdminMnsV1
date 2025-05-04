namespace AdminMnsV1.Models
{
   public class Student : User
    {
        public string? Nationality { get; set; }
        public string? SocialSecurityNumber { get; set; }
        public string? FranceTravailNumber { get; set; }
        public string? Role { get; set; } // Peut contenir "Candidat" ou "Stagiaire"
        //public string Photo { get; set; }
    }
}
