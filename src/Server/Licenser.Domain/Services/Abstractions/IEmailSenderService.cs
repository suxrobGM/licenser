using System.Threading.Tasks;

namespace Licenser.Domain.Services.Abstractions
{
    public interface IEmailSenderService
    {
        Task SendEmailAsync(string email, string subject, string htmlMessage);
    }
}