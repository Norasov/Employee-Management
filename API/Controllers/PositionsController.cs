using API.Extensions;
using Application.DTOs.Positions;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PositionsController(IPositionService service):ControllerBase
{
    private readonly IPositionService _service = service;

    [HttpGet]
    public async Task<IActionResult> GetPositions()
    {
        var positions = await _service.GetPositionsAsync();
        return this.ToActionResult(positions);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetPosition(int id)
    {
        var position = await _service.GetPositionAsync(id);
        return this.ToActionResult(position);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreatePositionDTO positionDTO)
    {
        var create = await _service.CreatePositionAsync(positionDTO);
        return this.ToActionResult(create);
    }
}