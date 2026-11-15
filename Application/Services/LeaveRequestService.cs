using System.Net.Cache;
using Application.DTOs.LeaveRequests;
using Application.HttpStatusCodes;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Enums;
using Domain.Models;

namespace Application.Services;

public class LeaveRequestService(
    ILeaveRequestRepository repository,
    IEmployeeRepository employeeRepository) : ILeaveRequestService
{
    private readonly ILeaveRequestRepository _repository = repository;
    private readonly IEmployeeRepository _employeeRepository = employeeRepository;
    public async Task<ServiceStatusCodes<bool>> CreateLeaveRequestAsync(CreateLeaveRequestDTO leaveRequestDTO)
    {
        if (leaveRequestDTO.EmployeeId <= 0)
            return ServiceStatusCodes<bool>.BadRequest("Идентификатор сотрудника должен быть больше нуля.");

        if (!Enum.IsDefined(leaveRequestDTO.LeaveType))
            return ServiceStatusCodes<bool>.BadRequest("Неверный тип заявки.");

        if (leaveRequestDTO.EndDate.Date < leaveRequestDTO.StartDate.Date)
            return ServiceStatusCodes<bool>.BadRequest("Дата окончания не может быть раньше даты начала.");

        if (leaveRequestDTO.StartDate.Date < DateTime.UtcNow.Date)
            return ServiceStatusCodes<bool>.BadRequest("Дата начала не может быть в прошлом.");

        var days = (leaveRequestDTO.EndDate.Date - leaveRequestDTO.StartDate.Date).Days + 1;
        if (leaveRequestDTO.LeaveType == LeaveType.Vacation && days > 28)
            return ServiceStatusCodes<bool>.BadRequest("Отпуск не может быть длиннее 28 календарных дней.");

        var employee = await _employeeRepository.GetEmployeeByIdAsync(leaveRequestDTO.EmployeeId);
        if (employee is null)
            return ServiceStatusCodes<bool>.NotFound("Сотрудник не найден.");

        if (!employee.IsActive)
            return ServiceStatusCodes<bool>.Conflict("Нельзя подать заявку за уволенного сотрудника.");

        LeaveRequest leaveRequest = new()
        {
            EmployeeId = leaveRequestDTO.EmployeeId,
            LeaveType = leaveRequestDTO.LeaveType,
            StartDate = leaveRequestDTO.StartDate.Date,
            EndDate = leaveRequestDTO.EndDate.Date,
            Reason = leaveRequestDTO.Reason,
            Status = LeaveStatus.Pending
        };

        var create = await _repository.CreateLeaveRequestAsync(leaveRequest);
        return ServiceStatusCodes<bool>.Created(create);
    }

    public async Task<ServiceStatusCodes<List<LeaveRequestListItemDTO>>> GetLeaveRequestsAsync(LeaveStatus? status)
    {
        if (status.HasValue && !Enum.IsDefined(status.Value))
            return ServiceStatusCodes<List<LeaveRequestListItemDTO>>.BadRequest("Неверный статус. Допустимо: 1 (Pending), 2 (Approved), 3 (Rejected).");

        var requests = await _repository.GetLeaveRequestsAsync(status);

        List<LeaveRequestListItemDTO> listItemDTOs = new();

        foreach (var r in requests)
        {
            var item = new LeaveRequestListItemDTO
            {
                Id = r.Id,
                EmployeeId = r.EmployeeId,
                EmployeeName = r.EmployeeFirstName + " " + r.EmployeeLastName,
                LeaveType = r.LeaveType,
                StartDate = r.StartDate,
                EndDate = r.EndDate,
                Status = r.Status,
                Reason = r.Reason,
                CreatedAt = r.CreatedAt
            };
            listItemDTOs.Add(item);
        }
        return ServiceStatusCodes<List<LeaveRequestListItemDTO>>.Ok(listItemDTOs);
    }

    public async Task<ServiceStatusCodes<bool>> ApproveLeaveRequestAsync(int id)
    {
        var change = await _repository.ChangeStatusAsync(id, LeaveStatus.Approved);
        return ServiceStatusCodes<bool>.Ok(change);
    }

    public async Task<ServiceStatusCodes<bool>> RejectLeaveRequestAsync(int id)
    {
        var change = await _repository.ChangeStatusAsync(id, LeaveStatus.Rejected);
        return ServiceStatusCodes<bool>.Ok(change);
    }

    private async Task<ServiceStatusCodes<bool>> ChangeStatusAsync(int id, LeaveStatus newStatus)
    {
        var leaveRequest = await _repository.GetLeaveRequestByIdAsync(id);
        if (leaveRequest is null)
            return ServiceStatusCodes<bool>.NotFound("Заявка не найдена.");

        if (leaveRequest.Status != LeaveStatus.Pending)
            return ServiceStatusCodes<bool>.Conflict("Рассмотреть можно только заявку в статусе Pending.");

        var changed = await _repository.ChangeStatusAsync(id, newStatus);

        return ServiceStatusCodes<bool>.NoContent();
    }
}