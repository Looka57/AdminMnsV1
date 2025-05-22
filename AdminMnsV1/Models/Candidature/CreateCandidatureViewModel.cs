using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering; // Nécessaire pour SelectListItem

namespace AdminMnsV1.Models.ViewModels
{
    public class CreateCandidatureViewModel
    {
        [Display(Name = "Nom du candidat")]
        [Required(ErrorMessage = "Le nom du candidat est requis.")]
        public string FirstName { get; set; }

        [Display(Name = "Prénom du candidat")]
        [Required(ErrorMessage = "Le prénom du candidat est requis.")]
        public string LastName { get; set; }

        [Display(Name = "Email du candidat")]
        [EmailAddress(ErrorMessage = "Format d'e-mail invalide.")]
        [Required(ErrorMessage = "L'email du candidat est requis.")]
        public string Email { get; set; }

        [Display(Name = "Classe Ciblée")]
        [Required(ErrorMessage = "La classe ciblée est requise.")]
        public int ClassId { get; set; } // Pour stocker l'ID de la classe sélectionnée

        //Liste des classes disponibles pour le dropDown
        public List<SelectListItem> AvailableClasses { get; set; } = new List<SelectListItem>();

        [Display(Name = "Documents requis pour le dossier")]
        [Required(ErrorMessage = "Veuillez sélectionner au moins un document.")]
        public List<int> SelectedDocumentTypeIds { get; set; } = new List<int>(); // Liste des IDs des types de documents sélectionnés

        // Liste des types de documents disponibles pour le dropDown
        public List<SelectListItem> AvailableDocumentTypes { get; set; } = new List<SelectListItem>();
    }
}