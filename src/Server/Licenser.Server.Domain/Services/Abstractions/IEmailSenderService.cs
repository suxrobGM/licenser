using System.Threading.Tasks;

namespace Licenser.Server.Domain.Services.Abstractions
{
    public interface IEmailSenderService
    {
        Task SendEmailAsync(string email, string subject, string htmlMessage);
    }
}