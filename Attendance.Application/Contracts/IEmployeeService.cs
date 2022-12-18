using Attendance.Application.Models.Employee;

namespace Attendance.Application.Contracts
{
    public interface IEmployeeService
    {
        Task<EmployeeDto> CreateAsync(CreateEmployeeDto data);
        Task<List<EmployeeDto>> GetAllAsync();
        Task<EmployeeDto> UpdateAsync(UpdateEmployeeDto data);
        Task DeleteAsync(Guid userId);
    }
}
