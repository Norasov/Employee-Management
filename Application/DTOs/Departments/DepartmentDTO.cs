using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Departments;

public class DepartmentDTO
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}
