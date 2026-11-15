using API.Extensions;
using Application.DTOs.Employees;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController(IEmployeeService service) : ControllerBase
{
    private readonly IEmployeeService _service = service;

    [HttpGet]
    public async Task<IActionResult> GetEmployees(
        int? departmentId, int? positionId, string? search, int page = 1, int pageSize = 20)
    {
        var employees = await _service.GetEmployeesAsync(
            departmentId, positionId, search, page, pageSize
        );

        return this.ToActionResult(employees);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetEmployeesById(int id)
    {
        var employees = await _service.GetEmployeeAsync(id);
        return this.ToActionResult(employees);
    }

    [HttpGet("{id:int}/leave-requests")]
    public async Task<IActionResult> GetEmployeeLeaveRequests(int id)
    {
        var employee = await _service.GetEmployeeLeaveRequestsAsync(id);
        return this.ToActionResult(employee);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(UpdateEmployeeDTO employeeDTO, int id)
    {
        var update = await _service.UpdateEmployeeAsync(employeeDTO, id);
        return this.ToActionResult(update);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateEmployeeDTO employeeDTO)
    {
        var create = await _service.CreateEmployeeAsync(employeeDTO);
        return this.ToActionResult(create);
    }

    [HttpPost("{id:int}/terminate")]
    public async Task<IActionResult> ChangeIsActive(int id)
    {
        var changed = await _service.ChangeIsActiveAsync(id);
        return this.ToActionResult(changed);
    }
}