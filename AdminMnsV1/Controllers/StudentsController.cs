using Microsoft.AspNetCore.Mvc;

namespace AdminMnsV1.Controllers
{
    public class StudentsController : Controller
    {
        public IActionResult Student()
        {
            return View();
        }
    }
}
