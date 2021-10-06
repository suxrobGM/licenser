using System.Threading.Tasks;

namespace Sms.Licensing.Domain.Services.Abstractions
{
    public interface IEmailSenderService
    {
        Task SendEmailAsync(string email, string subject, string htmlMessage);
    }
}