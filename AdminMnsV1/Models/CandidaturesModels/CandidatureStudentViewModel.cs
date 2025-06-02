using AdminMnsV1.Models.ViewModels;

namespace AdminMnsV1.Models.CandidaturesModels
{
    public class CandidatureStudentViewModel
    {
        // Propriétés de la candidature elle-même
        public int CandidatureId { get; set; }
        public string CandidatureStutus { get; set; }


        // Propriétés de l'étudiant (utilisateur lié à la candidature)
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public DateTime? BirthDate { get; set; } // DateTime? car elle peut être nullable
        public string StudentImage { get; set; } = "/images/default_student.png"; // Chemin vers l'image de l'étudiant


        // Propriétés liées à la classe
        public string ClassName { get; set; }


        // Propriétés pour la progression du dossier
        public int StudentValidationProgress { get; set; } // Pour la barre de progression côté étudiant
        public int MnsValidationProgress { get; set; } // Pour la barre de progression côté MNS (Admin)
        public string CandidatureStatus { get; set; }

        // Liste des notifications (si vous en avez)
        //public List<NotificationViewModel> Notifications { get; set; } = new List<NotificationViewModel>();

        // Liste des documents requis et leur statut
        public List<DocumentViewModel> RequiredDocuments { get; set; } = new List<DocumentViewModel>();


        //// ViewModel pour les notifications individuelles
        //public class NotificationViewModel
        //{
        //    public int NotificationId { get; set; }
        //    public string Message { get; set; }
        //    // Vous pourriez ajouter d'autres propriétés comme Date, Type de notification, etc.
        //}


        // ViewModel pour les documents individuels
    }
}
      
