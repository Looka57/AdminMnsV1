using Microsoft.AspNetCore.Mvc;

namespace AdminMnsV1.Controllers
{
    public class DelaysAbsStudentsController : Controller
    {
        // Utilisez l'attribut [Route] pour définir l'URL exacte pour cette action.
        // L'URL spécifiée ici sera https://localhost:7014/DelaysAbs/DelayAbsStudents
        [Route("DelaysAbs/DelayAbsStudents")]
        public IActionResult DelayAbsStudents()
        {
            // Spécifiez explicitement le chemin de la vue, car elle ne suit pas la convention du contrôleur.
            return View("~/Views/DelaysAbs/DelayAbsStudents.cshtml");
        }
    }
}