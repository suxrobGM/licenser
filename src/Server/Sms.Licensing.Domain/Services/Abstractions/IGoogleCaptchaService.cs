using System.Threading.Tasks;

namespace Sms.Licensing.Domain.Services.Abstractions
{
    public interface IGoogleCaptchaService
    {
        Task<bool> CheckCaptchaResponseAsync(string responseToken);
    }
}