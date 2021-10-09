using System.Threading.Tasks;

namespace Licenser.Domain.Services.Abstractions
{
    public interface IGoogleCaptchaService
    {
        Task<bool> CheckCaptchaResponseAsync(string responseToken);
    }
}