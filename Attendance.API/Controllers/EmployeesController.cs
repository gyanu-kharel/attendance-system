using Attendance.Application.Common;
using Attendance.Application.Contracts;
using Attendance.Application.Helpers;
using Attendance.Application.Models.Employee;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Attendance.UI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AuthorizeByRole(UserRoles.Admin)]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeesController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Create(CreateEmployeeDto data)
        {
            return Ok(await _employeeService.CreateAsync(data));
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> Get()
        {
            return Ok(await _employeeService.GetAllAsync());
        }

        [HttpPut]
        [Route("[action]")]
        public async Task<IActionResult> Update(UpdateEmployeeDto data)
        {
            return Ok(await _employeeService.UpdateAsync(data));
        }

        [HttpDelete]
        [Route("[action]")]
        public async Task<IActionResult> Delete(Guid employeeId)
        {
            await _employeeService.DeleteAsync(employeeId);
            return Ok();
        }
    }
}
