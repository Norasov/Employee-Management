using Application.Interfaces.Repositories;
using Dapper;
using Domain.Models;
using Infrastructure.Data;

namespace Infrastructure.Repositories;

public class DepartmentRepository(AppDbContext context) : IDepartmentRepository
{
    private readonly AppDbContext _context = context;
    public async Task<bool> CreateDepartmentAsync(Department department)
    {
        using var connection = _context.Connection();
        var query = @"insert into departments(name, description) values
        (@name, @description)";
        var create = await connection.ExecuteAsync(query, department);
        return create > 0;
    }

    public async Task<bool> DeleteDepartmentAsync(int id)
    {
        using var connection = _context.Connection();
        var query = "delete from departments where id = @id";
        var delete = await connection.ExecuteAsync(query, new { id });
        return delete > 0;
    }

    public async Task<Department?> GetDepartmentAsync(int id)
    {
        using var connection = _context.Connection();
        var query = "select * from departments where id = @id";
        return await connection.QuerySingleOrDefaultAsync<Department>(query, new { id });
    }

    public async Task<List<Department>> GetDepartmentsAsync()
    {
        using var connection = _context.Connection();
        var query = "select * from departments";
        var departments = await connection.QueryAsync<Department>(query);
        return departments.ToList();
    }

    public async Task<bool> IsNameExistsAsync(string name, int excludeId = 0)
    {
        using var connection = _context.Connection();

        var query = @"select exists(
        select 1 from departments                          
        where lower(name) = lower(@name)                 
        and id <> @excludeId)";

    return await connection.ExecuteScalarAsync<bool>(query, new { name, excludeId });
    }


    public async Task<bool> UpdateDepartmentAsync(Department department)
    {
        using var connection = _context.Connection();
        var query = @"update departments
        set name = @name,
        description = @description,
        createdat = @createdat
        where id = @id";

        var update = await connection.ExecuteAsync(query, department);
        return update > 0;
    }
}