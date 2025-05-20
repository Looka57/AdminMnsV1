using AdminMnsV1.Models; 
using System.Collections.Generic;

//Le DashboardViewModel est un ViewModel. Son rôle est de servir de pont entre le Contrôleur et la Vue. Il contient toutes les données dont la Vue a besoin pour s'afficher, et uniquement ces données.

namespace AdminMnsV1.ViewModels
{
    public class DashboardViewModel
    {
        // Propriété pour l'utilisateur connecté
        public User? LoggedInUser { get; set; }
        //Permet au contrôleur de passer l'objet User complet (l'utilisateur connecté) à la vue. C'est parfait pour afficher des informations.

        // Propriété pour la liste des cartes
        public List<CardModel> Cards { get; set; } = new List<CardModel>();
        // Une liste d'objets CardModel pour afficher vos cartes d'informations. L'initialisation avec new List<CardModel>() garantit que la liste ne sera jamais null, évitant ainsi les erreurs.

        // Propriétés pour les statistiques supplémentaires
        public int TotalClasses { get; set; } // Ce sont des propriétés simples pour passer des statistiques agrégées (nombre total de classes, d'étudiants, etc.) à la vue.
        public int TotalStudents { get; set; }
        public int NumberOfMen { get; set; }
        public int NumberOfWomen { get; set; }
    }
}