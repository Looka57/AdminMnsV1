using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AdminMnsV1.Models.Students
{


    //MODIFICATION ET AFFICHAGE  D'UN  STAGIAIRE
    public class StudentEditViewModel
    {
        public string UserId { get; set; } //Cette propriété stocke l'identifiant unique de l'étudiant. Essentielle pour identifier l'étudiant à modifier.

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

        public int ClassId { get; set; } // Pour stocker l'ID de la classe sélectionné
        public List<string> ClassesAttended { get; set; } = new List<string>(); // Initialiser la liste est une bonne pratique

        public DateTime CreationDate { get; set; }

        public DateTime BirthDate { get; set; }

        [MaxLength(50, ErrorMessage = "Le numéro de téléphone ne peut pas dépasser 50 caractères.")]
        public string Phone { get; set; }

        public string Nationality { get; set; }

        public string SocialSecurityNumber { get; set; }

        public string FranceTravailNumber { get; set; }

        public string Role { get; set; }

        public string? Photo { get; set; }


        //C' est un modèle de données conçu pour afficher les informations d'un étudiant existant dans un formulaire de modification et pour recueillir les mises à jour de ces informations.Il contient l'identifiant de l'étudiant pour la mise à jour et des attributs de validation pour les champs modifiables.
        // Le StudentEditViewModel est une classe C# utilisée comme modèle de vue dans ASP.NET MVC. Elle permet de transporter des données entre le contrôleur et la vue de manière ciblée (ici, pour les étudiants).

        // 🎯 Pourquoi utiliser un ViewModel au lieu de l'entité User directement ?
        // -- Tu évites d’exposer des champs sensibles.
        // -- Tu contrôles précisément les validations.
        // -- Tu simplifies l’affichage dans la vue (formulaires ciblés).
    }
}