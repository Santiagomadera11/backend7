using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace Syspharma.API.Services
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly EmailSettings _settings;
        private readonly ILogger<SmtpEmailSender> _logger;

        public SmtpEmailSender(IOptions<EmailSettings> options, ILogger<SmtpEmailSender> logger)
        {
            _settings = options.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(string to, string subject, string htmlBody)
        {
            try
            {
                using var message = new MailMessage
                {
                    From = new MailAddress(_settings.From),
                    Subject = subject,
                    Body = htmlBody,
                    IsBodyHtml = true
                };
                message.To.Add(new MailAddress(to));

                using var client = new SmtpClient(_settings.Host, _settings.Port)
                {
                    DeliveryMethod = SmtpDeliveryMethod.Network
                };

                // 1. PRIMERO: desactivar credenciales por defecto
                client.UseDefaultCredentials = false;

                // 2. SEGUNDO: asignar tus credenciales (contraseña de aplicación SIN ESPACIOS)
                client.Credentials = new NetworkCredential(_settings.User, _settings.Password);

                // 3. TERCERO: activar SSL (necesario para Gmail)
                client.EnableSsl = _settings.EnableSsl;

                await client.SendMailAsync(message);
                _logger.LogInformation("Email sent to {To} via {Host}:{Port}", to, _settings.Host, _settings.Port);
            }
            catch (SmtpException ex)
            {
                _logger.LogError(ex, "SMTP error sending email to {To}", to);
                throw;
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Unexpected error sending email to {To}", to);
                throw;
            }
        }
    }
}