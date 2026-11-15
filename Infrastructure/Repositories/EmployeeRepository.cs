using Application.DTOs.Employees;
using Application.DTOs.PageResults;
using Application.Interfaces.Repositories;
using Dapper;
using Domain.Models;
using Infrastructure.Data;

namespace Infrastructure.Repositories;

public class EmployeeRepository(AppDbContext context) : IEmployeeRepository
{
    private readonly AppDbContext _context = context;

    public async Task<bool> ChangeIsActiveAsync(int id)
    {
        using var connection = _context.Connection();
        var query = "update employees set isactive = false where id = @id";
        var changed = await connection.ExecuteAsync(query, new {id});
        return changed > 0;
    }


    public async Task<bool> CreateEmployeeAsync(Employee employee)
    {
        using var connection = _context.Connection();
        var query = @"insert into employees(firstname, lastname, email, phone, hiredate, salary, departmentid, positionid) values
        (@firstname, @lastname, @email, @phone, @hiredate, @salary, @departmentid, @positionid)";

        var create = await connection.ExecuteAsync(query, employee);
        return create > 0;
    }

    public async Task<EmployeeInfo?> GetEmployeeAsync(int id)
    {
        using var connection = _context.Connection();
        var query = @"select e.id, e.firstname, e.lastname e.email, e.phone
        e.hiredate, e.salary, e.isactive, e.departmentid, d.name as departmentname,
        e.positionid, p.title as positiontitle
        from employees e
        join departments t on d.id = e.departmentid
        join positions p on p.id = e.positionid
        where id = @id";

        return await connection.QuerySingleOrDefaultAsync<EmployeeInfo>(query, new { id });
    }

    public async Task<Employee?> GetEmployeeByIdAsync(int id)
    {
        using var connection = _context.Connection();
        var query = "select * from employees where id = @id";
        return await connection.QuerySingleOrDefaultAsync<Employee>(query, new {id});
    }

    public async Task<List<LeaveRequest>> GetEmployeeLeaveRequestsAsync(int id)
    {
        using var connection = _context.Connection();
        var query = @"select * from leaverequests
        where employeeid = @id
        order by startdate desc, id desc";
        var requests = await connection.QueryAsync<LeaveRequest>(query, new{id});
        return requests.ToList();
    }

    public async Task<PagedResult<EmployeeResponseDTO>> GetEmployeesAsync(int? departmentId, int? positionId, string? search, int page, int pageSize)
    {
        using var connection = _context.Connection();

        // 1. Получаем общее количество сотрудников
        var countQuery = @"
            SELECT COUNT(*)
            FROM employees e
            WHERE (@DepartmentId IS NULL OR e.departmentid = @DepartmentId)
              AND (@PositionId IS NULL OR e.positionid = @PositionId)
              AND (
                    @Search IS NULL
                    OR e.firstname ILIKE @Search
                    OR e.lastname ILIKE @Search
                  );";

        var totalCount = await connection.ExecuteScalarAsync<int>(
            countQuery,
            new
            {
                DepartmentId = departmentId,
                PositionId = positionId,
                Search = search == null ? null : $"%{search}%"
            });

        // 2. Получаем сотрудников с названиями отдела и должности
        var query = @"
            SELECT e.id, e.firstname, e.lastname, e.email,
                e.hiredate, e.salary, e.isactive,
                d.name AS ""DepartmentName"", p.title AS ""PositionTitle""
            FROM employees e
            INNER JOIN departments d ON d.id = e.departmentid
            INNER JOIN positions p ON p.id = e.positionid
            WHERE (@DepartmentId IS NULL OR e.departmentid = @DepartmentId)
              AND (@PositionId IS NULL OR e.positionid = @PositionId)
              AND (
                    @Search IS NULL
                    OR e.firstname ILIKE @Search
                    OR e.lastname ILIKE @Search
                  )
            ORDER BY e.id
            LIMIT @PageSize OFFSET @Offset;";

        var employees = await connection.QueryAsync<EmployeeResponseDTO>(
            query,
            new
            {
                DepartmentId = departmentId,
                PositionId = positionId,
                Search = search == null ? null : $"%{search}%",
                PageSize = pageSize,
                Offset = (page - 1) * pageSize
            });

        // 3. Возвращаем результат с пагинацией
        return new PagedResult<EmployeeResponseDTO>
        {
            Items = employees.ToList(),
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<bool> IsEmailExistsAsync(string email, int id)
    {
        using var connection = _context.Connection();
        var query = "select exists(select email from employees where lower(email) = lower(@email) and id <> @id)";
        return await connection.ExecuteScalarAsync<bool>(query, new { email, id });
    }


    public async Task<bool> UpdateEmployeeAsync(Employee employee)
    {
        using var connection = _context.Connection();
        var query = @"update employees
        set firstname = @firstname,
        lastname = @lastname,
        email = @email,
        phone = @phone,
        hiredate = @hiredate,
        salary = @salary,
        departmentid = @departmentid,
        positionid = @positionid
        where id = @id";

        var update = await connection.ExecuteAsync(query, employee);
        return update > 0;
    }
}
