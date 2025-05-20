// Controllers/ClassesController.cs
using Microsoft.AspNetCore.Mvc;
using AdminMnsV1.Services.Interfaces; // Pour IClassService
using AdminMnsV1.Models.ViewModels; // IMPORTANT : Pour ClassListViewModel
using System.Threading.Tasks;

namespace AdminMnsV1.Controllers
{
    public class ClassesController : Controller
    {
        private readonly IClassService _classService;

        public ClassesController(IClassService classService)
        {
            _classService = classService;
        }

        public async Task<IActionResult> Class()
        {
            ViewData["Title"] = "Classes";
            // Appelle la nouvelle méthode pour obtenir le ViewModel complet
            var viewModel = await _classService.GetClassListPageViewModelAsync();
            return View(viewModel); // Passe le ViewModel complet à la vue
        }
    }
}