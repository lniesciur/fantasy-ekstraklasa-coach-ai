using Microsoft.AspNetCore.Mvc;
using FantasyCoachAI.Application.Interfaces;
using FantasyCoachAI.Application.DTOs;
using FantasyCoachAI.Domain.Enums;
using FantasyCoachAI.Domain.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace FantasyCoachAI.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MatchesController : ControllerBase
{
    private readonly IMatchService _matchService;

    public MatchesController(IMatchService matchService)
    {
        _matchService = matchService;
    }

    /// <summary>
    /// List matches with filtering, sorting, and pagination
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetMatches(
        [FromQuery] int? gameweekId = null,
        [FromQuery] int? teamId = null,
        [FromQuery] string? status = null,
        [FromQuery] DateTime? dateFrom = null,
        [FromQuery] DateTime? dateTo = null,
        [FromQuery] string? sort = "match_date",
        [FromQuery] string? order = "asc",
        [FromQuery] int page = 1,
        [FromQuery] int limit = 50)
    {
        // Validate query params
        if (limit > 100)
            limit = 100;

        if (!new[] { "scheduled", "postponed", "cancelled", "played" }.Contains(status?.ToLower()))
            return BadRequest("Invalid status parameter");

        if (!new[] { "match_date", "gameweek_number" }.Contains(sort?.ToLower()))
            return BadRequest("Invalid sort parameter");

        if (!new[] { "asc", "desc" }.Contains(order?.ToLower()))
            return BadRequest("Invalid order parameter");

        MatchStatus? matchStatus = null;
        if (!string.IsNullOrEmpty(status) && Enum.TryParse<MatchStatus>(status, true, out var parsedStatus))
        {
            matchStatus = parsedStatus;
        }

        var filter = new MatchFilterDto
        {
            GameweekId = gameweekId,
            TeamId = teamId,
            Status = matchStatus,
            DateFrom = dateFrom,
            DateTo = dateTo,
            Sort = sort,
            Order = order,
            Page = page,
            Limit = limit
        };

        var matches = await _matchService.GetAllAsync(filter);

        return Ok(new 
        { 
            data = matches?.Data ?? new List<MatchDto>(),
            pagination = new 
            { 
                page = matches?.Pagination?.Page ?? page,
                limit = matches?.Pagination?.Limit ?? limit,
                total = matches?.Pagination?.Total ?? 0,
                pages = matches?.Pagination?.Pages ?? 0
            }
        });
    }

    /// <summary>
    /// Get specific match details
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetMatch(int id)
    {
        var match = await _matchService.GetByIdAsync(id);
        if (match == null)
            throw new NotFoundException("Match not found");

        return Ok(match);
    }

    /// <summary>
    /// Create new match
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateMatch([FromBody] CreateMatchCommand createMatchCommand)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var createdMatch = await _matchService.CreateAsync(createMatchCommand);
        return CreatedAtAction(nameof(GetMatch), new { id = createdMatch.Id }, createdMatch);
    }

    /// <summary>
    /// Update existing match
    /// </summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateMatch(int id, [FromBody] UpdateMatchCommand updateMatchCommand)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        updateMatchCommand.Id = id;

        var updatedMatch = await _matchService.UpdateAsync(updateMatchCommand);
        return Ok(updatedMatch);
    }
}
