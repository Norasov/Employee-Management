using Application.DTOs.Employees;
using Application.DTOs.PageResults;
using Domain.Models;

namespace Application.Interfaces.Repositories;

public interface IEmployeeRepository
{
    Task<bool> CreateEmployeeAsync(Employee employee);
    Task<bool> UpdateEmployeeAsync(Employee employee);
    Task<EmployeeInfo?> GetEmployeeAsync(int id);
    Task<Employee?> GetEmployeeByIdAsync(int id);
    Task<bool> ChangeIsActiveAsync(int id);
    public Task<PagedResult<EmployeeResponseDTO>> GetEmployeesAsync(
        int? departmentId,
        int? positionId,
        string? search,
        int page,
        int pageSize
    );

    Task<bool> IsEmailExistsAsync(string email, int id=0);
    Task<List<LeaveRequest>> GetEmployeeLeaveRequestsAsync(int id);
}
