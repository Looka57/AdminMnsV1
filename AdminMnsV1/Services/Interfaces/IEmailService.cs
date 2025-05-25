using System.Threading.Tasks;

namespace AdminMnsV1.Application.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string message);
    }
}