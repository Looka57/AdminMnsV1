using Microsoft.AspNetCore.Mvc;

namespace AdminMnsV1.Controllers
{
    public class ModificationsController : Controller
    {
        public IActionResult Modification()
        {
            return View("~/Views/Profils/Modification.cshtml");
        }
    }
}
