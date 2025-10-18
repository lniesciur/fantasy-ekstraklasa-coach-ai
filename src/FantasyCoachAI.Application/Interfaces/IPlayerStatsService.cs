using FantasyCoachAI.Application.DTOs;

namespace FantasyCoachAI.Application.Interfaces;

public interface IPlayerStatsService
{
    /// <summary>
    /// Get filtered player statistics
    /// </summary>
    /// <param name="filter">Filter parameters including gameweek_id (required)</param>
    /// <returns>Tuple of list of DTOs and total count</returns>
    Task<(List<PlayerStatsDto> data, int total)> GetStatsAsync(PlayerStatsFilterDto filter);

    /// <summary>
    /// Import player statistics from CSV file
    /// </summary>
    /// <param name="request">Import request containing file and gameweek ID</param>
    /// <returns>Import response with success status and counts</returns>
    Task<PlayerStatsImportResponseDto> ImportFromCsvAsync(PlayerStatsImportRequestDto request);
}
