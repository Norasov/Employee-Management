using Application.DTOs.Positions;
using Application.HttpStatusCodes;

namespace Application.Interfaces.Services;

public interface IPositionService
{
    Task<ServiceStatusCodes<bool>> CreatePositionAsync(CreatePositionDTO positionDTO);
    Task<ServiceStatusCodes<List<PositionDTO>>> GetPositionsAsync();
    Task<ServiceStatusCodes<PositionDTO>> GetPositionAsync(int id);
}