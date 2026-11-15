using Domain.Models;

namespace Application.Interfaces.Repositories;

public interface IPositionRepository
{
    Task<bool> CreatePositionAsync(Position position);
    Task<List<Position>> GetPositionsAsync();
    Task<Position?> GetPositionAsync(int id);
    Task<bool> IsPositionExistsAsync(string title);
}
