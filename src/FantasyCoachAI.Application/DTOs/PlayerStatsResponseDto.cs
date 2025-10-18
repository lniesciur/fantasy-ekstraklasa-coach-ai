using Microsoft.AspNetCore.Http;

namespace FantasyCoachAI.Application.DTOs;

/// <summary>
/// Response DTO for player statistics with pagination
/// </summary>
public class PlayerStatsResponseDto
{
    public List<PlayerStatsDto> Data { get; set; } = new();
    public PaginationDto Pagination { get; set; } = new();
}

/// <summary>
/// Request DTO for importing player statistics
/// </summary>
public class PlayerStatsImportRequestDto
{
    public IFormFile File { get; set; } = null!;
}
