using Application.DTOs.LeaveRequests;
using Application.HttpStatusCodes;
using Domain.Enums;

namespace Application.Interfaces.Services;

public interface ILeaveRequestService
{
    Task<ServiceStatusCodes<bool>> CreateLeaveRequestAsync(CreateLeaveRequestDTO leaveRequestDTO);
    Task<ServiceStatusCodes<List<LeaveRequestListItemDTO>>> GetLeaveRequestsAsync(LeaveStatus? status);
    Task<ServiceStatusCodes<bool>> ApproveLeaveRequestAsync(int id);
    Task<ServiceStatusCodes<bool>> RejectLeaveRequestAsync(int id);
}
