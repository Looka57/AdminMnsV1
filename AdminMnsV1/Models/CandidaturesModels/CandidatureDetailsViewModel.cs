using AdminMnsV1.Models.CandidaturesModels;
using AdminMnsV1.Models.Documents;
using AdminMnsV1.Models.Classes;
using AdminMnsV1.Models; // Pour User
using System.Collections.Generic;

namespace AdminMnsV1.Models.ViewModels
{
    public class CandidatureDetailsViewModel
    {
        public int CandidatureId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string ClassName { get; set; }
        public string CandidatureStatusName { get; set; }
        public DateTime CandidatureCreationDate { get; set; }
        public DateTime? CandidatureValidationDate { get; set; }
        public string UserProfilePictureUrl { get; set; } // Chemin vers l'image de profil de l'étudiant

        public List<DocumentViewModel> RequiredDocuments { get; set; } = new List<DocumentViewModel>();

        // Propriétés pour la progression du dossier (calculée)
        public int StudentValidationProgress { get; set; } // % de documents déposés par l'étudiant
        public int AdminValidationProgress { get; set; }   // % de documents validés par l'admin
    }

    public class DocumentViewModel
    {
        public int DocumentId { get; set; }
        public string DocumentTypeName { get; set; }
        public DateTime? DocumentDepositDate { get; set; }
        public string DocumentStatut { get; set; } // "Demandé", "Déposé", "Vérifié", "Validé", "Refusé"
        public string DocumentPath { get; set; } // Le lien pour vérifier le document

        // Ajoutez une propriété pour l'ID du type de document si nécessaire pour les actions
        public int DocumentTypeId { get; set; }
    }
}