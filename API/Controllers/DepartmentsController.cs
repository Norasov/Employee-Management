using API.Extensions;
using Application.DTOs.Departments;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DepartmentsController(IDepartmentService service) : ControllerBase
{
    private readonly IDepartmentService _service = service;
    [HttpGet]
    public async Task<IActionResult> GetDepartments()
    {
        var departments = await _service.GetDepartmentsAsync();
        return this.ToActionResult(departments);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetDepartment(int id)
    {
        var department = await _service.GetDepartmentAsync(id);
        return this.ToActionResult(department);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateDepartmentDTO departmentDTO)
    {
        var create = await _service.CreateDepartmentAsync(departmentDTO);
        return this.ToActionResult(create);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(UpdateDepartmentDTO departmentDTO, int id)
    {
        var update = await _service.UpdateDepartmentAsync(departmentDTO, id);
        return this.ToActionResult(update);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var delete = await _service.DeleteDepartmentAsync(id);
        return this.ToActionResult(delete);
    }
}