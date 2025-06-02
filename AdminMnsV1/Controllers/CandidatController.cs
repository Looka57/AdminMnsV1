using Microsoft.AspNetCore.Mvc;

namespace AdminMnsV1.Controllers
{
    public class CandidatController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
