// Dans ViewModels/DashboardViewModel.cs

using AdminMnsV1.Models; // Assurez-vous d'importer l'espace de noms de votre modèle User et CardModel
using System.Collections.Generic;

namespace AdminMnsV1.ViewModels
{
    public class DashboardViewModel
    {
        // Propriété pour l'utilisateur connecté
        public User? LoggedInUser { get; set; }

        // Propriété pour la liste des cartes
        public List<CardModel> Cards { get; set; } = new List<CardModel>();

        // Propriétés pour les statistiques supplémentaires
        public int TotalClasses { get; set; }
        public int TotalStudents { get; set; }
        public int NumberOfMen { get; set; }
        public int NumberOfWomen { get; set; }
    }
}