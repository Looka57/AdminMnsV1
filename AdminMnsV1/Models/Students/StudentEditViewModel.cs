using System.ComponentModel.DataAnnotations;

namespace AdminMnsV1.Models.Students
{
    public class StudentEditViewModel
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "Le nom est requis.")]
        [MaxLength(50, ErrorMessage = "Le nom ne peut pas dépasser 50 caractères.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Le prénom est requis.")]
        [MaxLength(50, ErrorMessage = "Le prénom ne peut pas dépasser 50 caractères.")]
        public string FirstName { get; set; }

        [MaxLength(50, ErrorMessage = "Le sexe ne peut pas dépasser 50 caractères.")]
        public string Sexe { get; set; }

        [EmailAddress(ErrorMessage = "Format d'e-mail invalide.")]
        [MaxLength(250, ErrorMessage = "L'e-mail ne peut pas dépasser 250 caractères.")]
        public string Email { get; set; }

        [MaxLength(150, ErrorMessage = "L'adresse ne peut pas dépasser 150 caractères.")]
        public string Address { get; set; }

        [MaxLength(50, ErrorMessage = "La ville ne peut pas dépasser 50 caractères.")]
        public string City { get; set; }

        public DateTime CreationDate { get; set; }
    

        public DateTime BirthDate { get; set; }

        [MaxLength(50, ErrorMessage = "Le numéro de téléphone ne peut pas dépasser 50 caractères.")]
        public string Phone { get; set; }

        public string Nationality { get; set; }

        public string SocialSecurityNumber { get; set; }

        public string FranceTravailNumber { get; set; }

        public string Role { get; set; }
        // Pas de propriété pour PasswordHash ni Discriminator ici !
        // Ni pour Photo si vous avez décidé de la supprimer.
    }
}