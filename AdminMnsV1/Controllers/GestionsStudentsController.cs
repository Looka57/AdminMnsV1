using Microsoft.AspNetCore.Mvc;

namespace AdminMnsV1.Controllers
{
    public class GestionsStudentsController : Controller
    {
        public IActionResult GestionStudent()
        {
            return View("~/Views/Profils/GestionStudent.cshtml");
        }
    }
}
