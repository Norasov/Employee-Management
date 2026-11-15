using Domain.Enums;

namespace Application.DTOs.LeaveRequests;

public class LeaveRequestListItemDTO
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = null!;   // "Имя Фамилия"
    public LeaveType LeaveType { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public LeaveStatus Status { get; set; }
    public string? Reason { get; set; }
    public DateTime CreatedAt { get; set; }

}
