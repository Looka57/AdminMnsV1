using System.ComponentModel.DataAnnotations;

namespace AdminMnsV1.Models.Absences
{
    public class AbsenceViewModel
    {
        public int AbsenceId { get; set; }

        [Display(Name = "Nom")]
        public string StudentLastName { get; set; } = string.Empty;
        [Display(Name = "Prénom")]
        public string StudentFirstName { get; set; } = string.Empty;
        [Display(Name = "Classe")]
        public string StudentClassName { get; set; } = string.Empty; // Sera rempli depuis SchoolClass

        [Display(Name = "Date de début")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        [Display(Name = "Date de fin")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }

        [Display(Name = "Durée")]
        public string Duration { get; set; } = string.Empty; // Propriété calculée

        [Display(Name = "Type")]
        public string ReasonAbsenceLabel { get; set; } = string.Empty;

        [Display(Name = "Statut")]
        public string JustificationStatus { get; set; } = string.Empty; // Statut (Validé, Refusé, En attente)

        public string StudentId { get; set; } = string.Empty; // ID de l'utilisateur
        public string JustificationFilePath { get; set; } = string.Empty; // Chemin du justificatif
    }
}
