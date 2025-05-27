using AdminMnsV1.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AdminMnsV1.Controllers
{
    public class TestEmailController : Controller
    {
        private readonly IEmailService _emailService;

        public TestEmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task<IActionResult> SendTest()
        {
            await _emailService.SendEmailAsync("test@fake.com", "Sujet test", "<b>Coucou depuis Mailtrap !</b>");
            return Content("E-mail envoyé !");
        }
    }

}
