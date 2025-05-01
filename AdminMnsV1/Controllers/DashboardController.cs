using Microsoft.AspNetCore.Mvc;

namespace AdminMnsV1.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
