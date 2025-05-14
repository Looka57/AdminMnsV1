using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering; // N'oubliez pas ceci
using AdminMnsV1.Models.Students; // Pour utiliser StudentEditViewModel

namespace AdminMnsV1.Models.ViewModels // Créez ou utilisez un dossier ViewModels si vous le souhaitez
{
    public class StudentListPageViewModel
    {
        // La liste des étudiants à afficher dans le tableau
        public IEnumerable<StudentEditViewModel> Students { get; set; }

        // La liste des classes disponibles pour les menus déroulants (dans les modales par exemple)
        public SelectList AvailableClasses { get; set; }


    }
}