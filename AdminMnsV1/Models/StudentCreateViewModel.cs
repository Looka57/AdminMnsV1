using System;
using System.ComponentModel.DataAnnotations;

namespace AdminMnsV1.Models
{
    public class StudentCreateViewModel
    {
        [Required(ErrorMessage = "Le nom est requis.")]
        [MaxLength(50, ErrorMessage = "Le nom ne peut pas dépasser 50 caractères.")]
        [Display(Name = "Nom")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Le prénom est requis.")]
        [MaxLength(50, ErrorMessage = "Le prénom ne peut pas dépasser 50 caractères.")]
        [Display(Name = "Prénom")]
        public string FirstName { get; set; }

        [MaxLength(50, ErrorMessage = "Le sexe ne peut pas dépasser 50 caractères.")]
        [Display(Name = "Sexe")]
        public string Sexe { get; set; }

        [EmailAddress(ErrorMessage = "Format d'e-mail invalide.")]
        [MaxLength(250, ErrorMessage = "L'e-mail ne peut pas dépasser 250 caractères.")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [MaxLength(150, ErrorMessage = "L'adresse ne peut pas dépasser 150 caractères.")]
        [Display(Name = "Adresse")]
        public string Address { get; set; }

        [MaxLength(50, ErrorMessage = "La ville ne peut pas dépasser 50 caractères.")]
        [Display(Name = "Ville")]
        public string City { get; set; }

        [Display(Name = "Date de naissance")]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }

        [MaxLength(50, ErrorMessage = "Le numéro de téléphone ne peut pas dépasser 50 caractères.")]
        [Display(Name = "Téléphone")]
        public string Phone { get; set; }

        [Display(Name = "Nationalité")]
        public string Nationality { get; set; }

        [Display(Name = "N° Sécurité Sociale")]
        public string SocialSecurityNumber { get; set; }

        [Display(Name = "N° France Travail")]
        public string FranceTravailNumber { get; set; }

        [Display(Name = "Rôle")]
        public string Role { get; set; } = "Candidat"; // Valeur par défaut

        // Vous pourriez choisir de ne pas inclure CreationDate ici, car elle sera définie lors de la création en base de données.
    }
}
