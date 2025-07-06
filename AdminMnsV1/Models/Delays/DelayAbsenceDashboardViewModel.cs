using AdminMnsV1.Models.Absences;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AdminMnsV1.Models.Delays
{
    public class DelayAbsenceDashboardViewModel
    {
        public IEnumerable<AbsenceViewModel> PendingAbsences { get; set; } = new List<AbsenceViewModel>();
        public IEnumerable<DelayViewModel> PendingDelays { get; set; } = new List<DelayViewModel>();

        // Pour les formulaires d'ajout dans l'offcanvas
        public AddAbsenceViewModel NewAbsence { get; set; } = new AddAbsenceViewModel();
        public AddDelayViewModel NewDelay { get; set; } = new AddDelayViewModel();

        // Pour les listes déroulantes/recherche dans les formulaires
        public IEnumerable<SelectListItem> AllReasonAbsences { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> AllReasonDelays { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> AllStudents { get; set; } = new List<SelectListItem>(); // Maintenant inclut le nom de la classe
    }
}
