using Application.DTOs.Positions;
using Application.HttpStatusCodes;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Models;

namespace Application.Services;

public class PositionService(IPositionRepository repository) : IPositionService
{
    private readonly IPositionRepository _repository = repository;
    public async Task<ServiceStatusCodes<bool>> CreatePositionAsync(CreatePositionDTO positionDTO)
    {
        if(string.IsNullOrWhiteSpace(positionDTO.Title))
            return ServiceStatusCodes<bool>.BadRequest("Вам необходимо заполнить название должности");
        
        if(positionDTO.MinSalary <= 0)
            return ServiceStatusCodes<bool>.BadRequest("Минимальная зарплата должна быть больше нуля.");

        if(positionDTO.MinSalary >= positionDTO.MaxSalary)
            return ServiceStatusCodes<bool>.BadRequest("Минимальная зарплата должна быть меньше максимальной");

        Position position = new()
        {
            Title = positionDTO.Title,
            MinSalary = positionDTO.MinSalary,
            MaxSalary = positionDTO.MaxSalary
        };

        var create = await _repository.CreatePositionAsync(position);
        return ServiceStatusCodes<bool>.Created(create);
    }

    public async Task<ServiceStatusCodes<PositionDTO>> GetPositionAsync(int id)
    {
        var position = await _repository.GetPositionAsync(id);
        if(position is null)
            return ServiceStatusCodes<PositionDTO>.NotFound("Должность не найдена.");

        PositionDTO positionDTO = new()
        {
            Title = position.Title,
            MinSalary = position.MinSalary,
            MaxSalary = position.MaxSalary
        };

        return ServiceStatusCodes<PositionDTO>.Ok(positionDTO);
    }

    public async Task<ServiceStatusCodes<List<PositionDTO>>> GetPositionsAsync()
    {
        var positions = await _repository.GetPositionsAsync();

        List<PositionDTO> positionDTOs = [];
        foreach (var position in positions)
        {
            PositionDTO positionDTO = new()
            {
                Title = position.Title,
                MinSalary = position.MinSalary,
                MaxSalary = position.MaxSalary
            };
            positionDTOs.Add(positionDTO);
        }

        return ServiceStatusCodes<List<PositionDTO>>.Ok(positionDTOs);
    }
}