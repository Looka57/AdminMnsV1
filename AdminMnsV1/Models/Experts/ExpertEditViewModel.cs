using System.ComponentModel.DataAnnotations;

namespace AdminMnsV1.Models.Experts
{
    public class ExpertEditViewModel
    {
        public int UserId { get; set; }

        [MaxLength(50, ErrorMessage = "Le nom ne peut pas dépasser 50 caractères.")]
        public string LastName { get; set; }


        [MaxLength(50, ErrorMessage = "Le prénom ne peut pas dépasser 50 caractères.")]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "Le sexe ne peut pas dépasser 50 caractères.")]
        public string Sexe { get; set; }


        [EmailAddress(ErrorMessage = "Format d'e-mail invalide.")]
        [MaxLength(250, ErrorMessage = "L'e-mail ne peut pas dépasser 250 caractères.")]
        public string Email { get; set; }


        //public string Password { get; set; }


        [MaxLength(150, ErrorMessage = "L'adresse ne peut pas dépasser 150 caractères.")]
        public string Address { get; set; }


        [MaxLength(50, ErrorMessage = "La ville ne peut pas dépasser 50 caractères.")]
        public string City { get; set; }

        [Required(ErrorMessage = "La spécialité est obligatoire.")] 
        [MaxLength(50, ErrorMessage = "Choisir une spécialité.")]
        [Display(Name = "Speciality")]
        public string Speciality { get; set; }

        public DateTime CreationDate { get; set; }

        //[MaxLength(50, ErrorMessage = "Le lieu ne peut pas dépasser 50 caractères.")]
        //public string BirthPlace { get; set; }

        public DateTime BirthDate { get; set; }


        [MaxLength(50, ErrorMessage = "Le numéro de téléphone ne peut pas dépasser 50 caractères.")]
        public string Phone { get; set; }

    }
}