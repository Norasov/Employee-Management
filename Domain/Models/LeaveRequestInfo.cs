using Domain.Enums;

namespace Domain.Models;

public class LeaveRequestInfo
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeFirstName { get; set; } = null!;   // из employees
    public string EmployeeLastName { get; set; } = null!;    // из employees
    public LeaveType LeaveType { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public LeaveStatus Status { get; set; }
    public string? Reason { get; set; }
    public DateTime CreatedAt { get; set; }

}
