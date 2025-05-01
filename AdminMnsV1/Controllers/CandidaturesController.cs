using Microsoft.AspNetCore.Mvc;

namespace AdminMnsV1.Controllers
{
    public class CandidaturesController : Controller
    {
        public IActionResult Candidature()
        {
            return View();
        }
    }
}
