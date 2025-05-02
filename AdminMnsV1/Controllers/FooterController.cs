using Microsoft.AspNetCore.Mvc;

namespace AdminMnsV1.Controllers
{
    public class FooterController : Controller
    {
        public IActionResult Footer()
        {
            return View();
        }
    }
}
