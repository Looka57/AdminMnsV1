using Microsoft.AspNetCore.Mvc;

namespace AdminMnsV1.Controllers
{
    public class ExpertsController : Controller
    {
        public IActionResult Expert()
        {
            return View();
        }
    }
}
