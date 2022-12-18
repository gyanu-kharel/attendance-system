using Attendance.Application.Models.Auth;

namespace Attendance.Application.Contracts
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginRequestDto data);
        Task<Guid> CreateUserAsync(string email);

        Task UpdateUserEmailAsync(Guid userId, string email);
    }
}
