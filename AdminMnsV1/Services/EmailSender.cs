using AdminMnsV1.Application.Services.Interfaces;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;

namespace AdminMnsV1.Infrastructure
{
    public class EmailSender : IEmailSender
    {
        private readonly IEmailService _emailService;

        public EmailSender(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            await _emailService.SendEmailAsync(email, subject, htmlMessage);
        }
    }
}
