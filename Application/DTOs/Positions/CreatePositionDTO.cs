namespace Application.DTOs.Positions;

public class CreatePositionDTO
{
    public string Title { get; set; } = null!;
    public decimal MinSalary { get; set; }
    public decimal MaxSalary { get; set; }
}