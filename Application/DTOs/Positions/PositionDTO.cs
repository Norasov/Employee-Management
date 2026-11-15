namespace Application.DTOs.Positions;

public class PositionDTO
{
    public string Title { get; set; } = null!;
    public decimal MinSalary { get; set; }
    public decimal MaxSalary { get; set; }

}
