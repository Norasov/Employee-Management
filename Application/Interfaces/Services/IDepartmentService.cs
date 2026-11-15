using Application.DTOs.Departments;
using Application.HttpStatusCodes;

namespace Application.Interfaces.Services;

public interface IDepartmentService
{
    Task<ServiceStatusCodes<bool>> CreateDepartmentAsync(CreateDepartmentDTO departmentDTO);
    Task<ServiceStatusCodes<List<DepartmentDTO>>> GetDepartmentsAsync();
    Task<ServiceStatusCodes<bool>> DeleteDepartmentAsync(int id);
    Task<ServiceStatusCodes<bool>> UpdateDepartmentAsync(UpdateDepartmentDTO departmentDTO, int id);
    Task<ServiceStatusCodes<DepartmentDTO>> GetDepartmentAsync(int id);
}
