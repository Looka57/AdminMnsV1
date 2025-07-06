using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AdminMnsV1.Models.Delays
{
    public class AddDelayViewModel
    {
        [Required(ErrorMessage = "L'heure et la date d'arrivée sont requises.")]
        [DataType(DataType.DateTime)]
        public DateTime ArrivalTime { get; set; }

        [Required(ErrorMessage = "Veuillez sélectionner un type de retard.")]
        [Display(Name = "Type de retard")]
        public int ReasonDelayId { get; set; }

        [Required(ErrorMessage = "L'ID du stagiaire est requis.")]
        public string StudentId { get; set; } = string.Empty; // Sera sélectionné via la liste déroulante

        public IFormFile? JustificatifFile { get; set; } // Pour le téléchargement de fichier

        // Pour les listes déroulantes dans le formulaire
        public IEnumerable<SelectListItem>? ReasonDelays { get; set; }
        public IEnumerable<SelectListItem>? Students { get; set; } // Contient les étudiants avec leur classe
    }
}
