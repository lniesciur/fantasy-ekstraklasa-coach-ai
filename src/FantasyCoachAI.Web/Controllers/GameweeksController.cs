using Microsoft.AspNetCore.Mvc;
using FantasyCoachAI.Application.Interfaces;
using FantasyCoachAI.Application.DTOs;
using FantasyCoachAI.Domain.Enums;
using FantasyCoachAI.Domain.Exceptions;
using FantasyCoachAI.Web.Filters;

namespace FantasyCoachAI.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GameweeksController : ControllerBase
{
    private readonly IGameweekService _gameweekService;

    public GameweeksController(IGameweekService gameweekService)
    {
        _gameweekService = gameweekService;
    }

    /// <summary>
    /// List all gameweeks with optional filtering and sorting
    /// </summary>
    [HttpGet]
    [AutoValidate]
    public async Task<IActionResult> GetGameweeks(
        [FromQuery] string? status = null,
        [FromQuery] string? sort = "number",
        [FromQuery] string? order = "asc")
    {
        var filter = new GameweekFilterDto
        {
            Status = !string.IsNullOrEmpty(status) && Enum.TryParse<GameweekStatus>(status, true, out var parsedStatus) 
                ? parsedStatus 
                : null,
            Sort = sort,
            Order = order
        };

        var gameweeks = await _gameweekService.GetGameweeksAsync(filter);

        return Ok(new 
        { 
            data = gameweeks,
            total = gameweeks?.Count ?? 0 
        });
    }

    /// <summary>
    /// Get specific gameweek with matches
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetGameweek(int id)
    {
        var gameweek = await _gameweekService.GetByIdAsync(id);
        if (gameweek == null)
            throw new NotFoundException("Gameweek not found");

        return Ok(gameweek);
    }

    /// <summary>
    /// Create new gameweek
    /// </summary>
    [HttpPost]
    [AutoValidate]
    public async Task<IActionResult> CreateGameweek([FromBody] CreateGameweekCommand createGameweekCommand)
    {
        var createdGameweek = await _gameweekService.CreateAsync(createGameweekCommand);
        return CreatedAtAction(nameof(GetGameweek), new { id = createdGameweek.Id }, createdGameweek);
    }

    /// <summary>
    /// Delete a gameweek
    /// </summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteGameweek(int id)
    {
        await _gameweekService.DeleteAsync(id);
        return NoContent();
    }
}
