using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AdminMnsV1.Models.DocumentTypes;
using Microsoft.AspNetCore.Mvc.Rendering; // Nécessaire pour SelectListItem

namespace AdminMnsV1.Models.ViewModels
{
    public class CreateCandidatureViewModel
    {
        [Display(Name = "Nom du candidat")]
        [Required(ErrorMessage = "Le nom du candidat est requis.")]
        public string LastName { get; set; }

        [Display(Name = "Prénom du candidat")]
        [Required(ErrorMessage = "Le prénom du candidat est requis.")]
        public string FirstName { get; set; }

        [Display(Name = "Email du candidat")]
        [EmailAddress(ErrorMessage = "Format d'e-mail invalide.")]
        [Required(ErrorMessage = "L'email du candidat est requis.")]
        public string Email { get; set; }


        [Display(Name = "Téléphone du candidat")]
        [Required(ErrorMessage ="Numero du candidat est requis")]
        public string PhoneNumber { get; set; }


        [Display(Name = "Statut du candidat")]
        [Required(ErrorMessage = "Statut du candidat est requis")]
        public string Statut { get; set; } = "Candidat";

        [Display(Name = "Date de naissance")]
        [DataType(DataType.Date)] // Pour le type input="date"
        public DateTime? BirthDate { get; set; } 


        [Display(Name = "Classe Ciblée")]
        [Required(ErrorMessage = "La classe ciblée est requise.")]
        public int ClassId { get; set; } // Pour stocker l'ID de la classe sélectionnée


        //Liste des classes disponibles pour le dropDown
        public List<SelectListItem> AvailableClasses { get; set; } = new List<SelectListItem>();


        // Liste des IDs des types de documents sélectionnés lors de la soumission.
        [Display(Name = "Documents requis pour le dossier")]
        [Required(ErrorMessage = "Veuillez sélectionner au moins un document.")]
        public List<int> RequiredDocumentTypeIds { get; set; } = new List<int>(); // Liste des IDs des types de documents sélectionnés

        // Cette propriété contiendra tous les types de documents disponibles pour que la vue puisse les afficher
        // (y compris ceux qui ne sont pas "par défaut" pour une classe spécifique, si vous en avez)
        public List<DocumentType> AllAvailableDocumentTypes { get; set; } = new List<DocumentType>();

        //// Elle contiendra les IDs des types de documents que l'utilisateur doit fournir pour cette candidature.
        //public List<int>? RequiredDocumentTypeIds { get; set; }

        // Optionnel : Pour afficher les types de documents disponibles (si tu les laisses choisir)
        public IEnumerable<SelectListItem>? AvailableDocumentTypes { get; set; }
    }
}