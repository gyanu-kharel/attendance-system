using Attendance.Application.Contracts;
using Attendance.Application.Exceptions;
using Attendance.Application.Models.Attendance;
using Attendance.Core.Entities;
using Attendance.DataAccess.Persistence.DbContext;
using Microsoft.EntityFrameworkCore;

namespace Attendance.Application.Implementations
{
    public class AttendanceService : IAttendanceService
    {
        private readonly AttendanceDbContext _dbContext;

        public AttendanceService(AttendanceDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<AttendanceDetail> CreatePunchInAsync(PunchInDto data)
        {
            var config = await _dbContext.AttendanceConfigs.Where(x => x.IsActive).FirstOrDefaultAsync();

            var currentTimespan = DateTime.UtcNow.TimeOfDay;

            // require remarks if punch in time is more than master punch in time
            if (currentTimespan > config.PunchInTime && string.IsNullOrEmpty(data.Remarks))
                throw new ValidationException("Employee must provide remarks for late punch in");

            var employee = await _dbContext.Employees.FirstOrDefaultAsync(x => x.ApplicationUserId == data.UserId);
            if (employee is null)
                throw new ValidationException("Employee not found");

            // check if the employee has already punchedIn
            var attendance = await _dbContext.AttendanceDetails
                .Where(x => x.EmployeeId == employee.Id && x.Date == DateTime.UtcNow && x.PunchInTime.HasValue)
                .FirstOrDefaultAsync();

            if (attendance is not null)
                throw new ValidationException("Employee has already punched in today");


            AttendanceDetail detail = new()
            {
                EmployeeId = employee.Id,
                LatePunchInRemarks = data.Remarks
            };

            _dbContext.AttendanceDetails.Add(detail);
            await _dbContext.SaveChangesAsync();

            // TODO: return a DTO instead?
            return detail;

        }

        public async Task<AttendanceDetail> CreatePunchOutAsync(PunchOutDto data)
        {
            var config = await _dbContext.AttendanceConfigs.FirstOrDefaultAsync(x => x.IsActive);

            var currentTimespan = DateTime.UtcNow.TimeOfDay;

            // requires remarks if punch out time is more than master punch out time
            if (currentTimespan > config.PunchOutTime && string.IsNullOrEmpty(data.Remarks))
                throw new ValidationException("Employee must provide remarks for late punch out");

            var employee = await _dbContext.Employees.FirstOrDefaultAsync(x => x.ApplicationUserId == data.UserId);
            if (employee is null)
                throw new ValidationException("Employee not found");

            var attendance = await _dbContext.AttendanceDetails
                .Where(x => x.EmployeeId == employee.Id && x.Date == DateTime.UtcNow)
                .FirstOrDefaultAsync();

            // check whether the employee punched in today, don't allow to punch out if there is no punch in
            if (!attendance.PunchInTime.HasValue)
                throw new ValidationException("PunchIn not found. Contact admin");

            // check whether the employee has already punched out
            if(attendance.PunchOutTime.HasValue)
                throw new ValidationException("Punch out is already recorded");

            attendance.PunchOutTime = DateTime.UtcNow.TimeOfDay;
            attendance.LatePunchOutRemarks = data.Remarks;

            _dbContext.AttendanceDetails.Update(attendance);
            await _dbContext.SaveChangesAsync();

            return attendance;
        }
    }
}
