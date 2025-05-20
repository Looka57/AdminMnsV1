// Controllers/FormulairesController.cs
using Microsoft.AspNetCore.Mvc;

namespace AdminMnsV1.Controllers
{
    public class FormulairesController : Controller
    {
        public IActionResult FormulaireExpert()
        {
            return View("~/Views/Experts/FormulaireExpert.cshtml");
        }
    }
}