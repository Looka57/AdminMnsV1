using AdminMnsV1.Models;
using Microsoft.AspNetCore.Mvc;

namespace AdminMnsV1.Controllers
{
    public class FormulairesController : Controller
    {
        public IActionResult Formulaire()
        {
            return View("~/Views/Students/Formulaire.cshtml");
        }

        public IActionResult FormulaireExpert()
        {
            return View("~/Views/Experts/FormulaireExpert.cshtml");
        }

        public IActionResult FormulaireAddStudent()
        {
            return View("Formulaire", new StudentCreateViewModel()); // Renvoie la vue Formulaire.cshtml avec le ViewModel
        }

    }
}
