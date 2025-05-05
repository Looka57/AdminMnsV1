using AdminMnsV1.Models;
using AdminMnsV1.Models.Students;
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

        //AJOUT UN NOUVEAU STAGIAIRE
        public IActionResult FormulaireAddStudent()
        {
            return View("Formulaire", new ExpertCreateViewModel()); // Renvoie la vue Formulaire.cshtml avec le ViewModel
        }

        //SUPPRIME UN STAGIAIRE
        public IActionResult FormulaireDeletedStudent()
        {
            return View("Formulaire", new ExpertCreateViewModel()); // Renvoie la vue Formulaire.cshtml avec le ViewModel
        }
    }
}
