using Application.Interfaces.Repositories;
using Dapper;
using Domain.Enums;
using Domain.Models;
using Infrastructure.Data;

namespace Infrastructure.Repositories;

public class LeaveRequestRepository(AppDbContext context) : ILeaveRequestRepository
{
    private readonly AppDbContext _context = context;
    public async Task<bool> CreateLeaveRequestAsync(LeaveRequest leaveRequest)
    {
        using var connection = _context.Connection();
        var query = @"insert into leaverequests(employeeid, leavetype, startdate, enddate, status, reason) values
        (@employeeid, @leavetype, @startdate, @enddate, @status, @reason)";
        var create = await connection.ExecuteAsync(query, leaveRequest);
        return create > 0;
    }
    public async Task<List<LeaveRequestInfo>> GetLeaveRequestsAsync(LeaveStatus? status)
    {
        using var connection = _context.Connection();

        var query = @"select l.id, l.employeeid,
               e.firstname as EmployeeFirstName,
               e.lastname as EmployeeLastName,
               cast(l.leavetype as int) as leavetype,
               l.startdate, l.enddate,
               cast(l.status as int) as status,
               l.reason, l.createdat
        from leaverequests l
        join employees e on e.id = l.employeeid
        where l.status = @status
        order by l.createdat desc, l.id desc";

        if (status.HasValue)
        {
            var filtered = await connection.QueryAsync<LeaveRequestInfo>(query,new { status = ((int)status.Value).ToString() });
            return filtered.ToList();
        }

        var all = await connection.QueryAsync<LeaveRequestInfo>(query);
        return all.ToList();
    }

    public async Task<LeaveRequest?> GetLeaveRequestByIdAsync(int id)
    {
        using var connection = _context.Connection();
        var query = @"select id, employeeid, cast(leavetype as int) as leavetype, startdate, enddate,
        cast(status as int) as status,
        reason, createdat
            from leaverequests
            where id = @id";
        return await connection.QuerySingleOrDefaultAsync<LeaveRequest>(query, new { id });
    }

    public async Task<bool> ChangeStatusAsync(int id, LeaveStatus status)
    {
        using var connection = _context.Connection();

        var query = "update leaverequests set status = @status where id = @id and status = '1'";

        var update = await connection.ExecuteAsync(query, new { id, status = ((int)status).ToString() });
        return update > 0;
    }

    public async Task<bool> HasOverlappingLeaveRequestAsync(int employeeId, DateTime startDate, DateTime endDate)
    {
        using var connection = _context.Connection();

        var query = @"select exists(
        select 1 from leaverequests
        where employeeid = @employeeId
          and status in ('1', '2')
          and startdate <= @endDate
          and enddate >= @startDate)";

        return await connection.ExecuteScalarAsync<bool>(query, new { employeeId, startDate, endDate });
    }
}
