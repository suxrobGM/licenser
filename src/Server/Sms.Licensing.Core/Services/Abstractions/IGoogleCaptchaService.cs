using System.Threading.Tasks;

namespace Sms.Licensing.Core.Services.Abstractions
{
    public interface IGoogleCaptchaService
    {
        Task<bool> CheckCaptchaResponseAsync(string responseToken);
    }
}