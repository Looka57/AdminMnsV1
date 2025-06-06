using System; // Pour DateTime
using AdminMnsV1.Models.ViewModels;

namespace AdminMnsV1.Models.ViewModels
{
    public class DocumentViewModel
    {
        public int DocumentId { get; set; }
        public string? DocumentName { get; set; } // Ajouté pour correspondre à votre mappage
        public string? DocumentTypeName { get; set; }
        public DateTime? DocumentDepositDate { get; set; }
        public string? DocumentStatut { get; set; } // Ajouté car vous l'avez mentionné
        public string? DocumentPath { get; set; }
        public int DocumentTypeId { get; set; } // ID du type de document
        public DateTime? UploadDate { get; set; } // Renommé de 'object' à 'DateTime?' pour être plus précis
        public bool IsVerified { get; set; }
        public DateTime DepositDate { get; internal set; }


        //public class DocumentViewModel
        //{
        //    public int DocumentId { get; set; } // L'ID du CandidatureDocument ou du Document lui-même
        //    public string DocumentTypeName { get; set; } // Ex: "Lettre de motivation", "RIB"
        //    public DateTime? UploadDate { get; set; } // Date de téléchargement du document par l'étudiant
        //    public string DocumentPath { get; set; } // Chemin d'accès au fichier sur le serveur (pour le bouton "Vérifier")
        //    public bool IsVerified { get; set; } // Indique si le document a été validé par le MNS
        //}
    }
}