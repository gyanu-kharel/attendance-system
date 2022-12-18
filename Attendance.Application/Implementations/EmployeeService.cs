using Attendance.Application.Contracts;
using Attendance.Application.Models.Employee;
using Attendance.Core.Entities;
using Attendance.DataAccess.Persistence.DbContext;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Attendance.Application.Implementations
{
    public class EmployeeService : IEmployeeService
    {
        private readonly AttendanceDbContext _dbContext;
        private readonly IAuthService _authService;

        public EmployeeService(AttendanceDbContext dbContext, IAuthService authService)
        {
            _dbContext = dbContext;
            _authService = authService;
        }
        public async Task<EmployeeDto> CreateAsync(CreateEmployeeDto data)
        {
            if (string.IsNullOrEmpty(data.Email))
                throw new ValidationException("Email cannot be blank");

            if (string.IsNullOrEmpty(data.Name))
                throw new ValidationException("Name cannot be blank");

            // perfrom regex email validation for more security

            var newUserId = await _authService.CreateUserAsync(data.Email);

            // TODO: Fetch currently loggedIn user's detail to assign value on CreatedBy
            var employee = new Employee()
            {
                Name = data.Name,
                ApplicationUserId = newUserId,
                CreatedBy = "loggedInUser"
            };

            await _dbContext.Employees.AddAsync(employee);
            await _dbContext.SaveChangesAsync();

            return new EmployeeDto(employee.Id, employee.Name, data.Email, employee.CreatedOn.ToShortDateString(), employee.IsActive);
        }

        public async Task<List<EmployeeDto>> GetAllAsync()
        {
           return await _dbContext.Employees.Select(x =>  new EmployeeDto(
                x.Id,
                x.Name,
                x.ApplicationUser.Email,
                x.CreatedOn.ToShortDateString(),
                x.IsActive
            ))
            .ToListAsync();
        }

        public async Task<EmployeeDto> UpdateAsync(UpdateEmployeeDto data)
        {
            var employee = await _dbContext.Employees.FirstOrDefaultAsync(x => x.Id == data.EmployeeId);
            if (employee is null)
                throw new ValidationException("Employee not found");

            if(!string.IsNullOrEmpty(data.Name))
                employee.Name = data.Name;

            if(!string.IsNullOrEmpty(data.Email))
                await _authService.UpdateUserEmailAsync(employee.ApplicationUserId, data.Email);

            _dbContext.Employees.Update(employee);
            await _dbContext.SaveChangesAsync();

            return new EmployeeDto(
                employee.Id,
                employee.Name,
                employee.ApplicationUser?.Email,
                employee.CreatedOn.ToShortDateString(),
                employee.IsActive
                );
        }

        public async Task DeleteAsync(Guid employeeId)
        {
            var employee = await _dbContext.Employees.FirstOrDefaultAsync(x => x.Id == employeeId);

            if (employee is null)
                throw new ValidationException("Employee not found");

            if (!employee.IsActive)
                throw new ValidationException("Employee is already deleted");

            employee.IsActive = false;
            _dbContext.Employees.Update(employee);
            await _dbContext.SaveChangesAsync();
        }
    }
}
