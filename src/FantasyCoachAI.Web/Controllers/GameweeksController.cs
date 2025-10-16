using Microsoft.AspNetCore.Mvc;
using FantasyCoachAI.Application.Interfaces;
using FantasyCoachAI.Application.DTOs;
using FantasyCoachAI.Domain.Enums;
using FantasyCoachAI.Domain.Exceptions;
using System.ComponentModel.DataAnnotations;

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
    public async Task<IActionResult> GetGameweeks(
        [FromQuery] string? status = null,
        [FromQuery] string? sort = "number",
        [FromQuery] string? order = "asc")
    {
        // Validate query params
        if (!new[] { "upcoming", "current", "completed" }.Contains(status?.ToLower()))
            return BadRequest("Invalid status parameter");

        if (!new[] { "number", "start_date" }.Contains(sort?.ToLower()))
            return BadRequest("Invalid sort parameter");

        if (!new[] { "asc", "desc" }.Contains(order?.ToLower()))
            return BadRequest("Invalid order parameter");

        GameweekStatus? gameweekStatus = null;
        if (!string.IsNullOrEmpty(status) && Enum.TryParse<GameweekStatus>(status, true, out var parsedStatus))
        {
            gameweekStatus = parsedStatus;
        }

        var gameweeks = await _gameweekService.GetGameweeksAsync(new GameweekFilterDto
        {
            Status = gameweekStatus,
            Sort = sort,
            Order = order
        });

        return Ok(new 
        { 
            data = gameweeks,
            total = gameweeks?.Count ?? 0 
        });
    }

    /// <summary>
    /// Get current active gameweek
    /// </summary>
    [HttpGet("current")]
    public async Task<IActionResult> GetCurrentGameweek()
    {
        // Get all gameweeks and find the current one
        var gameweeks = await _gameweekService.GetGameweeksAsync(new GameweekFilterDto
        {
            Status = GameweekStatus.Current
        });

        var gameweek = gameweeks?.FirstOrDefault();
        if (gameweek == null)
            throw new NotFoundException("No active gameweek");

        return Ok(gameweek);
    }

    /// <summary>
    /// Get specific gameweek with matches
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetGameweek(int id)
    {
        var gameweek = await _gameweekService.GetByIdWithMatchesAsync(id);
        if (gameweek == null)
            throw new NotFoundException("Gameweek not found");

        return Ok(gameweek);
    }

    /// <summary>
    /// Create new gameweek
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateGameweek([FromBody] CreateGameweekCommand createGameweekCommand)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var createdGameweek = await _gameweekService.CreateAsync(createGameweekCommand);
        return CreatedAtAction(nameof(GetGameweek), new { id = createdGameweek.Id }, createdGameweek);
    }

    /// <summary>
    /// Update existing gameweek
    /// </summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateGameweek(int id, [FromBody] UpdateGameweekCommand updateGameweekCommand)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        updateGameweekCommand.Id = id;

        var updatedGameweek = await _gameweekService.UpdateAsync(updateGameweekCommand);
        return Ok(updatedGameweek);
    }
}
