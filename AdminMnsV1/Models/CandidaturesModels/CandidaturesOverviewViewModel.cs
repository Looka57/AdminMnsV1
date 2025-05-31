using System; // Required for DateTime
using System.Collections.Generic;
using AdminMnsV1.Models.DocumentTypes; // Assurez-vous que DocumentType est dans ce namespace
using Microsoft.AspNetCore.Mvc.Rendering; // Pour SelectListItem
using CandidatureModel = AdminMnsV1.Models.CandidaturesModels.Candidature;



namespace AdminMnsV1.Models.ViewModels
{
    public class CandidaturesOverviewViewModel
    {
        // Propriétés du formulaire de création de candidature (anciennement de CreateCandidatureViewModel)
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
       
        public DateTime? BirthDate { get; set; }
        public string Statut { get; set; } = "Candidat";
        public int ClassId { get; set; }
        public IEnumerable<SelectListItem> AvailableClasses { get; set; }
        public List<int> RequiredDocumentTypeIds { get; set; }
        public IEnumerable<DocumentType> AllAvailableDocumentTypes { get; set; }


        // Propriétés pour les listes de candidatures (pour les accordéons)
        // Celles-ci doivent être de type Candidature, car c'est ce que votre contrôleur leur assigne.
        public IEnumerable<CandidatureModel> CandidaturesEnCours { get; set; }
        public IEnumerable<CandidatureModel> CandidaturesValidees { get; set; }
        public IEnumerable<CandidatureModel> CandidaturesRefusees { get; set; }

        public CandidaturesOverviewViewModel()
        {
            // Initialisation des listes pour éviter les NullReferenceException en Razor
            AvailableClasses = new List<SelectListItem>();
            RequiredDocumentTypeIds = new List<int>();
            AllAvailableDocumentTypes = new List<DocumentType>();

            // Initialisation des listes de candidatures pour les accordéons
            CandidaturesEnCours = new List<CandidatureModel>();
            CandidaturesValidees = new List<CandidatureModel>();
            CandidaturesRefusees = new List<CandidatureModel>();


        }
    }
}