// Controllers/StudentsController.cs
using Microsoft.AspNetCore.Mvc;
using AdminMnsV1.Models.Students;
using AdminMnsV1.Models.ViewModels;
using AdminMnsV1.Services.Interfaces; // Utilise l'interface du service
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering; // Nécessaire si tu manipules SelectList directement dans le contrôleur (ici, pour les erreurs)

namespace AdminMnsV1.Controllers
{
    public class StudentsController : Controller
    {
        private readonly IStudentService _studentService; // Injecte l'interface du service

        public StudentsController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        // Récupérer les stagiaires pour l'affichage
        public async Task<IActionResult> Student()
        {
            var pageViewModel = await _studentService.GetStudentListPageViewModelAsync();
            return View(pageViewModel);
        }

        // Afficher le formulaire de création (GET)
        public async Task<IActionResult> Create()
        {
            var viewModel = await _studentService.GetStudentCreateViewModelAsync();
            return View("~/Views/Students/Formulaire.cshtml", viewModel);
        }

        // Gérer la soumission du formulaire de création (POST)
        [HttpPost]
        public async Task<IActionResult> Create(StudentCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var (result, errorMessage) = await _studentService.CreateStudentAsync(model);

                if (result.Succeeded)
                {
                    TempData["SuccesMessage"] = "Le nouveau stagiaire a été créé et inscrit à la classe sélectionnée avec succès.";
                    return RedirectToAction("Student");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, errorMessage);
                    // Re-popule le SelectList si la création échoue pour réafficher le formulaire
                    model.AvailableClasses = (await _studentService.GetStudentCreateViewModelAsync()).AvailableClasses;
                    return View("~/Views/Students/Formulaire.cshtml", model);
                }
            }

            // Si ModelState n'est pas valide dès le départ
            model.AvailableClasses = (await _studentService.GetStudentCreateViewModelAsync()).AvailableClasses;
            return View("~/Views/Students/Formulaire.cshtml", model);
        }

        // Modifier un stagiaire (POST)
        [HttpPost]
        public async Task<IActionResult> Modify(StudentEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErreurMessage"] = "Informations invalides fournies.";
                return RedirectToAction("Student");
            }

            var (result, errorMessage) = await _studentService.UpdateStudentAsync(model);

            if (result.Succeeded)
            {
                TempData["SuccesMessage"] = $"Le stagiaire {model.FirstName} {model.LastName} a été mis à jour avec succès.";
                return RedirectToAction("Student");
            }
            else
            {
                TempData["ErreurMessage"] = errorMessage;
                return RedirectToAction("Student");
            }
        }

        // Supprimer un stagiaire (POST)
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var (result, errorMessage) = await _studentService.SoftDeleteStudentAsync(id);

            if (result.Succeeded)
            {
                TempData["SuccesMessage"] = "L'utilisateur a été marqué comme supprimé."; // Le nom complet est géré dans le service
                return RedirectToAction(nameof(Student));
            }
            else
            {
                TempData["ErreurMessage"] = errorMessage;
                return RedirectToAction(nameof(Student));
            }
        }
    }
}