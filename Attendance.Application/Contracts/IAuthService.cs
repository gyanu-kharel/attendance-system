using Attendance.Application.Models.Auth;
using Attendance.Core.Entities;

namespace Attendance.Application.Contracts
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginRequestDto data);
        Task<Guid> CreateUserAsync(string email);
        Task UpdateUserEmailAsync(Guid userId, string email);
        Task<bool> EmailExistsAsync(string email); 
    }
}
