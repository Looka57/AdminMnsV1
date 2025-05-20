// Controllers/ClassesController.cs
using Microsoft.AspNetCore.Mvc;
using AdminMnsV1.Models; // Pour CardModel
using AdminMnsV1.Services.Interfaces; // Pour IClassService
using System.Threading.Tasks;
// Retire les using qui ne sont plus nécessaires, comme AdminMnsV1.Data, Microsoft.EntityFrameworkCore, etc.

namespace AdminMnsV1.Controllers
{
    public class ClassesController : Controller
    {
        private readonly IClassService _classService; // Injecte l'interface du service

        public ClassesController(IClassService classService)
        {
            _classService = classService;
        }

        public async Task<IActionResult> Class()
        {
            // Appelle le service pour obtenir directement la liste des CardModel
            var classCards = await _classService.GetClassCardModelsAsync();
            return View(classCards);
        }

        // Si tu as d'autres actions (Create, Edit, Delete) pour les classes,
        // tu devras les refactoriser de la même manière, en appelant des méthodes du _classService.
    }
}