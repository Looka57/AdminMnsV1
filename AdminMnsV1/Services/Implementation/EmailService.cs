using AdminMnsV1.Application.Services.Interfaces;
using AdminMnsV1.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System; // Pour Console.WriteLine et Exception
using System.Threading.Tasks;

namespace AdminMnsV1.Application.Services.Implementation
{
    public class EmailService : IEmailService
    {
        private readonly SmtpSettings _smtpSettings;

        public EmailService(IOptions<SmtpSettings> smtpSettings)
        {
            _smtpSettings = smtpSettings.Value;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_smtpSettings.FromName, _smtpSettings.FromAddress));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;
            email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = message };

            using var smtp = new SmtpClient();
            try
            {
                await smtp.ConnectAsync(_smtpSettings.Host, _smtpSettings.Port, _smtpSettings.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None);
                // Note: Pour certains serveurs (ex: Gmail), si vous utilisez l'authentification à deux facteurs, vous devrez générer un mot de passe d'application.
                await smtp.AuthenticateAsync(_smtpSettings.Username, _smtpSettings.Password);
                await smtp.SendAsync(email);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'envoi de l'e-mail : {ex.Message}");
                // Pour le débogage, vous pouvez relancer l'exception ou la loguer plus en détail
                throw;
            }
            finally
            {
                await smtp.DisconnectAsync(true);
            }
        }
    }
}