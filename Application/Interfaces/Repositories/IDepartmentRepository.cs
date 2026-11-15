using Domain.Models;

namespace Application.Interfaces.Repositories;

public interface IDepartmentRepository
{
    Task<bool> CreateDepartmentAsync(Department department);
    Task<List<Department>> GetDepartmentsAsync();
    Task<Department?> GetDepartmentAsync(int id);
    Task<bool> DeleteDepartmentAsync(int id);
    Task<bool> UpdateDepartmentAsync(Department department);
    Task<bool> IsNameExistsAsync(string name, int excludeId = 0);
}
