using System.Threading.Tasks;

namespace Syspharma.API.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string to, string subject, string htmlBody);
    }
}