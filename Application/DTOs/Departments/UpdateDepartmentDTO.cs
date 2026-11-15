namespace Application.DTOs.Departments;

public class UpdateDepartmentDTO
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}
