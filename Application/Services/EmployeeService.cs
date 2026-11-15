using Application.DTOs.Employees;
using Application.DTOs.PageResults;
using Application.HttpStatusCodes;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Models;

namespace Application.Services;

public class EmployeeService(
    IEmployeeRepository repository,
    IDepartmentRepository departmentRepository,
    IPositionRepository positionRepository) : IEmployeeService
{
    private readonly IEmployeeRepository _repository = repository;
    private readonly IDepartmentRepository _departmentRepository = departmentRepository;
    private readonly IPositionRepository _positionRepository = positionRepository;

    public async Task<ServiceStatusCodes<bool>> ChangeIsActiveAsync(int id)
    {
        var employee = await _repository.GetEmployeeByIdAsync(id);
        if (employee is null)
            return ServiceStatusCodes<bool>.NotFound("Сотрудник не найден.");

        if (!employee.IsActive)
            return ServiceStatusCodes<bool>.Conflict("Сотрудник уже уволен.");

        var changed = await _repository.ChangeIsActiveAsync(id);

        return ServiceStatusCodes<bool>.Ok(changed);
    }

    public async Task<ServiceStatusCodes<bool>> CreateEmployeeAsync(CreateEmployeeDTO employeeDTO)
    {
        if (string.IsNullOrWhiteSpace(employeeDTO.FirstName))
            return ServiceStatusCodes<bool>.BadRequest("Имя сотрудника не должно быть пустым.");

        if (string.IsNullOrWhiteSpace(employeeDTO.LastName))
            return ServiceStatusCodes<bool>.BadRequest("Фамилия сотрудника не должна быть пустой.");

        if (string.IsNullOrWhiteSpace(employeeDTO.Email))
            return ServiceStatusCodes<bool>.BadRequest("Почта сотрудника не должна быть пустой.");

        if (await _repository.IsEmailExistsAsync(employeeDTO.Email.Trim()))
            return ServiceStatusCodes<bool>.Conflict("Сотрудник с такой почтой уже существует.");

        if (employeeDTO.HireDate > DateTime.Now)
            return ServiceStatusCodes<bool>.BadRequest("Дата приема не должно быть позже сегодняшнего дня.");

        if (employeeDTO.Salary < 0)
            return ServiceStatusCodes<bool>.BadRequest("Зарплата не должна быть меньше нуля");

        if (employeeDTO.DepartmentId < 0)
            return ServiceStatusCodes<bool>.BadRequest("Идентификатор отдела не должно быть меньше нуля.");

        if (employeeDTO.PositionId < 0)
            return ServiceStatusCodes<bool>.BadRequest("Идентификатор должности не должно быть меньше нуля.");

        var department = await _departmentRepository.GetDepartmentAsync(employeeDTO.DepartmentId);
        if (department is null)
            return ServiceStatusCodes<bool>.NotFound("Отдел не найден.");

        var position = await _positionRepository.GetPositionAsync(employeeDTO.PositionId);
        if (position is null)
            ServiceStatusCodes<bool>.NotFound("Должность не найдена.");

        if (employeeDTO.Salary < position.MinSalary || employeeDTO.Salary > position.MaxSalary)
            return ServiceStatusCodes<bool>.BadRequest(
                $"Зарплата для этой должности должна быть от {position.MinSalary} до {position.MaxSalary}.");

        Employee employee = new()
        {
            FirstName = employeeDTO.FirstName,
            LastName = employeeDTO.LastName,
            Email = employeeDTO.Email,
            Phone = employeeDTO.Phone,
            HireDate = employeeDTO.HireDate,
            Salary = employeeDTO.Salary,
            DepartmentId = employeeDTO.DepartmentId,
            PositionId = employeeDTO.PositionId
        };

        var create = await _repository.CreateEmployeeAsync(employee);

        return ServiceStatusCodes<bool>.Created(create);
    }

    public async Task<ServiceStatusCodes<EmployeeDTO>> GetEmployeeAsync(int id)
    {
        var employee = await _repository.GetEmployeeAsync(id);
        if (employee is null)
            return ServiceStatusCodes<EmployeeDTO>.NotFound("Сотрудник не найден.");

        EmployeeDTO employeeDTO = new()
        {
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Email = employee.Email,
            Phone = employee.Phone,
            HireDate = employee.HireDate,
            Salary = employee.Salary,
            IsActive = employee.IsActive,
            DepartmentId = employee.DepartmentId,
            DepartmentName = employee.DepartmentName,
            PositionId = employee.PositionId,
            PositionTitle = employee.PositionTitle
        };

        return ServiceStatusCodes<EmployeeDTO>.Ok(employeeDTO);
    }

    public async Task<ServiceStatusCodes<List<EmployeeLeaveRequestDTO>>> GetEmployeeLeaveRequestsAsync(int id)
    {
        var employee = await _repository.GetEmployeeByIdAsync(id);
        if (employee is null)
            return ServiceStatusCodes<List<EmployeeLeaveRequestDTO>>.NotFound("Сотрудник не найден.");

        var requests = await _repository.GetEmployeeLeaveRequestsAsync(id);

        List<EmployeeLeaveRequestDTO> leaveRequestDTOs = [];
        foreach (var request in requests)
        {
            EmployeeLeaveRequestDTO leaveRequestDTO = new()
            {
                LeaveType = request.LeaveType,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Status = request.Status,
                Reason = request.Reason,
                CreatedAt = request.CreatedAt
            };
            leaveRequestDTOs.Add(leaveRequestDTO);
        }

        return ServiceStatusCodes<List<EmployeeLeaveRequestDTO>>.Ok(leaveRequestDTOs);
    }

    public async Task<ServiceStatusCodes<PagedResult<EmployeeResponseDTO>>> GetEmployeesAsync(int? departmentId, int? positionId, string? search, int page, int pageSize)
    {
        if (page < 1)
            return ServiceStatusCodes<PagedResult<EmployeeResponseDTO>>
                .BadRequest("Page должен быть не меньше 1.");

        if (pageSize < 1 || pageSize > 100)
            return ServiceStatusCodes<PagedResult<EmployeeResponseDTO>>
                .BadRequest("PageSize должен быть от 1 до 100");

        search = string.IsNullOrWhiteSpace(search) ? null : search.Trim();

        var employees = await _repository.GetEmployeesAsync(
            departmentId, positionId, search, page, pageSize);

        return ServiceStatusCodes<PagedResult<EmployeeResponseDTO>>.Ok(employees);

    }

    public async Task<ServiceStatusCodes<bool>> UpdateEmployeeAsync(UpdateEmployeeDTO employeeDTO, int id)
    {
        if (string.IsNullOrWhiteSpace(employeeDTO.FirstName))
            return ServiceStatusCodes<bool>.BadRequest("Имя сотрудника не должно быть пустым.");

        if (string.IsNullOrWhiteSpace(employeeDTO.LastName))
            return ServiceStatusCodes<bool>.BadRequest("Фамилия сотрудника не должна быть пустой.");

        if (string.IsNullOrWhiteSpace(employeeDTO.Email))
            return ServiceStatusCodes<bool>.BadRequest("Почта сотрудника не должна быть пустой.");

        if (await _repository.IsEmailExistsAsync(employeeDTO.Email.Trim(), id))
            return ServiceStatusCodes<bool>.Conflict("Сотрудник с такой почтой уже существует.");

        if (employeeDTO.HireDate > DateTime.Now)
            return ServiceStatusCodes<bool>.BadRequest("Дата приема не должно быть позже сегодняшнего дня.");

        if (employeeDTO.Salary < 0)
            return ServiceStatusCodes<bool>.BadRequest("Зарплата не должна быть меньше нуля");

        if (employeeDTO.DepartmentId < 0)
            return ServiceStatusCodes<bool>.BadRequest("Идентификатор отдела не должно быть пустым.");

        if (employeeDTO.PositionId < 0)
            return ServiceStatusCodes<bool>.BadRequest("Идентификатор должности не должно быть пустым.");


        var department = await _departmentRepository.GetDepartmentAsync(employeeDTO.DepartmentId);
        if (department is null)
            return ServiceStatusCodes<bool>.NotFound("Отдел не найден.");

        var position = await _positionRepository.GetPositionAsync(employeeDTO.PositionId);
        if (position is null)
            ServiceStatusCodes<bool>.NotFound("Должность не найдена.");

        if (employeeDTO.Salary < position.MinSalary || employeeDTO.Salary > position.MaxSalary)
            return ServiceStatusCodes<bool>.BadRequest(
                $"Зарплата для этой должности должна быть от {position.MinSalary} до {position.MaxSalary}.");

        var employee = await _repository.GetEmployeeByIdAsync(id);
        if (employee is null)
            return ServiceStatusCodes<bool>.NotFound("Сотрудник не найден.");

        employee.FirstName = employeeDTO.FirstName;
        employee.LastName = employeeDTO.LastName;
        employee.Email = employeeDTO.Email;
        employee.Phone = employeeDTO.Phone;
        employee.HireDate = employeeDTO.HireDate;
        employee.Salary = employeeDTO.Salary;
        employee.DepartmentId = employeeDTO.DepartmentId;
        employee.PositionId = employeeDTO.PositionId;

        var udpate = await _repository.UpdateEmployeeAsync(employee);

        return ServiceStatusCodes<bool>.NoContent();
    }
}
