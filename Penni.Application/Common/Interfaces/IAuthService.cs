using Penni.Application.DTOs;
using System.Threading.Tasks;

namespace Penni.Application.Common.Interfaces
{
    public interface IAuthService
    {
        Task<LoginJwtResponseDto> LoginAsync(LoginDto loginDto);
        Task RegisterAsync(RegisterDto registerDto);
        Task<string> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto);
        Task ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
    }
}
