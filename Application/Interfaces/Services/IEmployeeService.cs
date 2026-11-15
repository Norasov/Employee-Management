using Application.DTOs.Employees;
using Application.DTOs.PageResults;
using Application.HttpStatusCodes;

namespace Application.Interfaces.Services;

public interface IEmployeeService
{
    Task<ServiceStatusCodes<bool>> CreateEmployeeAsync(CreateEmployeeDTO employeeDTO);

    Task<ServiceStatusCodes<PagedResult<EmployeeResponseDTO>>> GetEmployeesAsync(
        int? departmentId,
        int? positionId,
        string? search,
        int page,
        int pageSize
    );

    Task<ServiceStatusCodes<EmployeeDTO>> GetEmployeeAsync(int id);
    Task<ServiceStatusCodes<bool>> UpdateEmployeeAsync(UpdateEmployeeDTO employeeDTO, int id);
    Task<ServiceStatusCodes<bool>> ChangeIsActiveAsync(int id);
    Task<ServiceStatusCodes<List<EmployeeLeaveRequestDTO>>> GetEmployeeLeaveRequestsAsync(int id);
}
