using Application.DTOs.Departments;
using Application.HttpStatusCodes;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Models;

namespace Application.Services;

public class DepartmentService(IDepartmentRepository repository) : IDepartmentService
{
    private readonly IDepartmentRepository _repository = repository;

    public async Task<ServiceStatusCodes<bool>> CreateDepartmentAsync(CreateDepartmentDTO departmentDTO)
    {
        if (string.IsNullOrWhiteSpace(departmentDTO.Name))
            return ServiceStatusCodes<bool>.BadRequest("Вам необходимо заполнить название отдела.");

        var name = departmentDTO.Name.Trim();

        if (await _repository.IsNameExistsAsync(name))
            return ServiceStatusCodes<bool>.Conflict("Отдел с таким название уже существует.");
        
        Department department = new()
        {
            Name = name,
            Description = departmentDTO.Description,
        };


        var create = await _repository.CreateDepartmentAsync(department);
        return ServiceStatusCodes<bool>.Created(create);
    }

    public async Task<ServiceStatusCodes<bool>> DeleteDepartmentAsync(int id)
    {
        var delete = await _repository.DeleteDepartmentAsync(id);
        if (!delete)
            return ServiceStatusCodes<bool>.NotFound("Отдел не найден.");

        return ServiceStatusCodes<bool>.NoContent();
    }

    public async Task<ServiceStatusCodes<DepartmentDTO>> GetDepartmentAsync(int id)
    {
        var department = await _repository.GetDepartmentAsync(id);
        if (department is null)
            return ServiceStatusCodes<DepartmentDTO>.NotFound("Отдел не найден.");

        DepartmentDTO departmentDTO = new()
        {
            Name = department.Name,
            Description = department.Description,
            CreatedAt = department.CreatedAt
        };

        return ServiceStatusCodes<DepartmentDTO>.Ok(departmentDTO);
    }

    public async Task<ServiceStatusCodes<List<DepartmentDTO>>> GetDepartmentsAsync()
    {
        var departments = await _repository.GetDepartmentsAsync();

        List<DepartmentDTO> departmentDTOs = [];
        foreach (var department in departments)
        {
            DepartmentDTO departmentDTO = new()
            {
                Name = department.Name,
                Description = department.Description,
                CreatedAt = department.CreatedAt
            };
            departmentDTOs.Add(departmentDTO);
        }

        return ServiceStatusCodes<List<DepartmentDTO>>.Ok(departmentDTOs);
    }

    public async Task<ServiceStatusCodes<bool>> UpdateDepartmentAsync(UpdateDepartmentDTO departmentDTO, int id)
    {
        if (string.IsNullOrWhiteSpace(departmentDTO.Name))
            return ServiceStatusCodes<bool>.BadRequest("Название отдела не может быть пустым.");

        var department = await _repository.GetDepartmentAsync(id);

        if (department is null)
            return ServiceStatusCodes<bool>.NotFound("Отдел не найден.");

        var name = departmentDTO.Name.Trim();

        if (await _repository.IsNameExistsAsync(name, excludeId: id))
            return ServiceStatusCodes<bool>.Conflict("Отдел с таким названием уже существует.");

        department.Name = name;
        department.Description = departmentDTO.Description;

        var updated = await _repository.UpdateDepartmentAsync(department);

        return updated
            ? ServiceStatusCodes<bool>.NoContent()
            : ServiceStatusCodes<bool>.NotFound("Отдел не найден.");
    }
}