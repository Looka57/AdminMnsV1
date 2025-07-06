using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AdminMnsV1.Models.Absences
{
    public class AddAbsenceViewModel
    {
        [Required(ErrorMessage = "La date de début est requise.")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "La date de fin est requise.")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Veuillez sélectionner un type d'absence.")]
        [Display(Name = "Type d'absence")]
        public int ReasonAbsenceId { get; set; }

        [Required(ErrorMessage = "L'ID du stagiaire est requis.")]
        public string StudentId { get; set; } = string.Empty; // Sera sélectionné via la liste déroulante

        public IFormFile? JustificatifFile { get; set; } // Pour le téléchargement de fichier

        // Pour les listes déroulantes dans le formulaire
        public IEnumerable<SelectListItem>? ReasonAbsences { get; set; }
        public IEnumerable<SelectListItem>? Students { get; set; } // Contient les étudiants avec leur classe
    }
}
