using Microsoft.AspNetCore.Mvc;
using FantasyCoachAI.Application.Interfaces;
using FantasyCoachAI.Application.DTOs;
using FantasyCoachAI.Web.Filters;

namespace FantasyCoachAI.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlayerStatsController : ControllerBase
{
    private readonly IPlayerStatsService _statsService;

    public PlayerStatsController(IPlayerStatsService statsService)
    {
        _statsService = statsService;
    }

    /// <summary>
    /// Get player statistics for specific gameweek with optional filtering and pagination
    /// </summary>
    [HttpGet]
    [AutoValidate]
    public async Task<IActionResult> GetPlayerStats([FromQuery] PlayerStatsFilterDto filter)
    {
        var (data, total) = await _statsService.GetStatsAsync(filter);

        var response = new PlayerStatsResponseDto
        {
            Data = data,
            Pagination = new PaginationDto
            {
                Page = filter.Page,
                Limit = filter.Limit,
                Total = total,
                Pages = (total + filter.Limit - 1) / filter.Limit
            }
        };

        return Ok(response);
    }

    /// <summary>
    /// Import player statistics from CSV file
    /// </summary>
    [HttpPost("import")]
    [RequestFormLimits(MultipartBodyLengthLimit = 10485760)] // 10MB
    [Consumes("multipart/form-data")]
    [AutoValidate]
    public async Task<IActionResult> ImportStats([FromForm] PlayerStatsImportRequestDto request)
    {
        var result = await _statsService.ImportFromCsvAsync(request);

        if (!result.Success)
            throw new InvalidOperationException($"Import failed: {string.Join(", ", result.Errors)}");

        return Created(string.Empty, result);
    }
}
