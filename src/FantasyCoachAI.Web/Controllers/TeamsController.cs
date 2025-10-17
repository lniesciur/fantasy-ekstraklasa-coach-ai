using Microsoft.AspNetCore.Mvc;
using FantasyCoachAI.Application.Interfaces;
using FantasyCoachAI.Application.DTOs;
using FantasyCoachAI.Domain.Exceptions;
using FantasyCoachAI.Web.Filters;

namespace FantasyCoachAI.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TeamsController : ControllerBase
{
    private readonly ITeamService _teamService;

    public TeamsController(ITeamService teamService)
    {
        _teamService = teamService;
    }

    /// <summary>
    /// List all teams with optional filtering and sorting
    /// </summary>
    [HttpGet]
    [AutoValidate]
    public async Task<IActionResult> GetTeams(
        [FromQuery] string? sort = "name",
        [FromQuery] string? order = "asc",
        [FromQuery] bool? isActive = true,
        [FromQuery] string? shortCode = null)
    {
        var filter = new TeamFilterDto
        {
            Sort = sort,
            Order = order,
            IsActive = isActive,
            ShortCode = shortCode
        };

        var teams = await _teamService.GetTeamsAsync(filter);

        return Ok(new
        {
            data = teams,
            total = teams?.Count ?? 0
        });
    }

    /// <summary>
    /// Get specific team details
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetTeam(int id)
    {
        if (id <= 0)
            throw new ArgumentException("Invalid team ID");

        var team = await _teamService.GetByIdAsync(id);
        if (team == null)
            throw new NotFoundException("Team not found");

        return Ok(team);
    }

    /// <summary>
    /// Create new team
    /// </summary>
    [HttpPost]
    [AutoValidate]
    public async Task<IActionResult> CreateTeam([FromBody] CreateTeamCommand createTeamCommand)
    {
        var createdTeam = await _teamService.CreateAsync(createTeamCommand);
        return CreatedAtAction(nameof(GetTeam), new { id = createdTeam.Id }, createdTeam);
    }

    /// <summary>
    /// Update existing team
    /// </summary>
    [HttpPut("{id:int}")]
    [AutoValidate]
    public async Task<IActionResult> UpdateTeam(int id, [FromBody] UpdateTeamCommand updateTeamCommand)
    {
        updateTeamCommand.Id = id;

        var updatedTeam = await _teamService.UpdateAsync(updateTeamCommand);
        return Ok(updatedTeam);
    }
}
