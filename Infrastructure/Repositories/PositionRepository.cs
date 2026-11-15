using Application.Interfaces.Repositories;
using Dapper;
using Domain.Models;
using Infrastructure.Data;

namespace Infrastructure.Repositories;

public class PositionRepository(AppDbContext context) : IPositionRepository
{
    private readonly AppDbContext _context = context;
    public async Task<bool> CreatePositionAsync(Position position)
    {
        using var connection = _context.Connection();
        var query = @"insert into positions (title, minsalary, maxsalary) values
        (@title,@minsalary,@maxsalary)";

        var create = await connection.ExecuteAsync(query, position);
        return create > 0;
    }

    public async Task<Position?> GetPositionAsync(int id)
    {
        using var connection = _context.Connection();
        var query = "select * from positions where id = @id";
        return await connection.QuerySingleOrDefaultAsync<Position>(query, new{id});
    }

    public async Task<List<Position>> GetPositionsAsync()
    {
        using var connection = _context.Connection();
        var query = "select * from positions";
        var positions = await connection.QueryAsync<Position>(query);
        return positions.ToList();
    }

    public async Task<bool> IsPositionExistsAsync(string title)
    {
        using var connection = _context.Connection();
        var query = "select exists (select title from positions where lower(title) = lower(@title))";
        return await connection.ExecuteScalarAsync<bool>(query, new{title});   
    }
}