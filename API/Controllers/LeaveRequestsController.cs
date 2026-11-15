using System.Security.Principal;
using API.Extensions;
using Application.DTOs.LeaveRequests;
using Application.Interfaces.Services;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LeaveRequestsController(ILeaveRequestService service) : ControllerBase
{
    private readonly ILeaveRequestService _service = service;
    
    [HttpPost]
    public async Task<IActionResult> Create(CreateLeaveRequestDTO leaveRequestDTO)
    {
        var create = await _service.CreateLeaveRequestAsync(leaveRequestDTO);
        return this.ToActionResult(create);
    }

    [HttpGet("leave-request-status")]
    public async Task<IActionResult> GetLeaveRequests(LeaveStatus? status)
    {
        var leaveRequests = await _service.GetLeaveRequestsAsync(status);
        return this.ToActionResult(leaveRequests);
    }

    [HttpPost("{id:int}/approve")]
    public async Task<IActionResult> Approve(int id)
    {
        var approve = await _service.ApproveLeaveRequestAsync(id);
        return this.ToActionResult(approve);
    }

    [HttpPost("{id:int}/reject")]
    public async Task<IActionResult> Reject(int id)
    {
        var reject = await _service.RejectLeaveRequestAsync(id);
        return this.ToActionResult(reject);
    }
}