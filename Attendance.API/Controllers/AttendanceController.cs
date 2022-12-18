using Attendance.Application.Common;
using Attendance.Application.Contracts;
using Attendance.Application.Helpers;
using Attendance.Application.Models.Attendance;
using Microsoft.AspNetCore.Mvc;

namespace Attendance.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AuthorizeByRole(UserRoles.Employee)]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceService _attendanceService;

        public AttendanceController(IAttendanceService attendanceService)
        {
            _attendanceService = attendanceService;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> PunchIn(PunchInDto data)
        {
            return Ok(await _attendanceService.CreatePunchInAsync(data));
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> PunchOut(PunchOutDto data)
        {
            return Ok(await _attendanceService.CreatePunchOutAsync(data));
        }
    }
}
