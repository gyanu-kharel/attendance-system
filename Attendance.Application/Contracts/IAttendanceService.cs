using Attendance.Application.Models.Attendance;
using Attendance.Core.Entities;

namespace Attendance.Application.Contracts
{
    public interface IAttendanceService
    {
        Task<AttendanceDetail> CreatePunchInAsync(PunchInDto data);
        Task<AttendanceDetail> CreatePunchOutAsync(PunchOutDto data);
    }
}
