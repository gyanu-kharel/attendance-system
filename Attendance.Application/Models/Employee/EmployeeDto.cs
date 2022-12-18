namespace Attendance.Application.Models.Employee
{
    public record EmployeeDto(Guid Id, string Name, string Email, string CreatedOn, bool IsActive);
}
