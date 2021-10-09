using System.Threading.Tasks;

namespace Licenser.Server.Domain.Services.Abstractions
{
    public interface IGoogleCaptchaService
    {
        Task<bool> CheckCaptchaResponseAsync(string responseToken);
    }
}