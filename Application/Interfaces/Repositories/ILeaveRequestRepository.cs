using Domain.Enums;
using Domain.Models;

namespace Application.Interfaces.Repositories;

public interface ILeaveRequestRepository
{
    Task<bool> CreateLeaveRequestAsync(LeaveRequest leaveRequest);
    Task<List<LeaveRequestInfo>> GetLeaveRequestsAsync(LeaveStatus? status);
    Task<LeaveRequest?> GetLeaveRequestByIdAsync(int id);
    Task<bool> ChangeStatusAsync(int id, LeaveStatus status);
    Task<bool> HasOverlappingLeaveRequestAsync(int employeeId, DateTime startDate, DateTime endDate);
}
