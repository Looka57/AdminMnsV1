// AdminMnsV1.ViewModels/CandidatOnboardingViewModel.cs

using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering; // Si tu as des SelectList

namespace AdminMnsV1.ViewModels
{
    public class CandidatOnboardingViewModel
    {
        public string UserId { get; set; } // L'ID de l'utilisateur connecté

        // Propriétés de base existantes (pré-remplies et souvent en lecture seule)
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Prénom")]
        public string FirstName { get; set; }

        [Display(Name = "Nom")]
        public string LastName { get; set; }

        [Display(Name = "Téléphone")]
        public string Phone { get; set; } // Peut être mis à jour si nécessaire

        [Display(Name = "Date de naissance")]
        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; } // Peut être mis à jour si nécessaire

        // Propriétés que le CANDIDAT doit OBLIGATOIREMENT RENSEIGNER OU METTRE À JOUR
        [Required(ErrorMessage = "Veuillez spécifier votre sexe.")]
        [Display(Name = "Sexe")]
        public string Sexe { get; set; }

        [Required(ErrorMessage = "Veuillez spécifier votre nationalité.")]
        [StringLength(100)]
        [Display(Name = "Nationalité")]
        public string Nationality { get; set; }

        [Required(ErrorMessage = "L'adresse est requise.")]
        [StringLength(255)]
        [Display(Name = "Adresse")]
        public string Address { get; set; }

        [Required(ErrorMessage = "La ville est requise.")]
        [StringLength(100)]
        [Display(Name = "Ville")]
        public string City { get; set; }


        [Required(ErrorMessage = "Le numéro de Sécurité Sociale est requis.")]
        [StringLength(20)]
        [Display(Name = "N° Sécurité Sociale")]
        public string SocialSecurityNumber { get; set; }

        [Required(ErrorMessage = "Le numéro France Travail est requis.")]
        [StringLength(20)]
        [Display(Name = "N° France Travail")]
        public string FranceTravailNumber { get; set; }

        public DateTime CandidatureCreationDate { get; set; }



        [Display(Name = "Photo de profil")]
        [DataType(DataType.Upload)]
        public IFormFile? PhotoFile { get; set; } // Peut être nul si la photo est facultative lors de la mise à jour

        // Nouvelle propriété pour stocker le chemin/URL de la photo existante à afficher
        public string? ExistingPhotoPath { get; set; }




        // Pour les listes déroulantes comme le sexe si tu veux les afficher dans la vue
        public List<SelectListItem> SexesOptions { get; } = new List<SelectListItem>
        {
            new SelectListItem { Value = "", Text = "Sélectionner" },
            new SelectListItem { Value = "Male", Text = "Masculin" },
            new SelectListItem { Value = "Female", Text = "Féminin" }
        };
    }
}