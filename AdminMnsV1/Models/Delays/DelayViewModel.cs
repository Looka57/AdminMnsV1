using System.ComponentModel.DataAnnotations;

namespace AdminMnsV1.Models.Delays
{
    public class DelayViewModel
    {
        public int DelayId { get; set; }

        [Display(Name = "Nom")]
        public string StudentLastName { get; set; } = string.Empty;
        [Display(Name = "Prénom")]
        public string StudentFirstName { get; set; } = string.Empty;
        [Display(Name = "Classe")]
        public string StudentClassName { get; set; } = string.Empty; // Sera rempli depuis SchoolClass

        [Display(Name = "Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DelayDate { get; set; } // Juste la partie date de ArrivalTime

        [Display(Name = "Heure d'arrivée")]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime ArrivalTime { get; set; } // DateTime complet, mais affichage de l'heure seulement

        [Display(Name = "Motif")]
        public string ReasonDelayLabel { get; set; } = string.Empty;

        [Display(Name = "Statut")]
        public string JustificationStatus { get; set; } = string.Empty; // Statut (Validé, Refusé, En attente)

        public string StudentId { get; set; } = string.Empty; // ID de l'utilisateur
        public string JustificationFilePath { get; set; } = string.Empty; // Chemin du justificatif
    }

}
