namespace FantasyCoachAI.Domain.Interfaces;

public interface IPlayerStatsRepository
{
    /// <summary>
    /// Get filtered player statistics with pagination
    /// Returns tuple of data and total count
    /// </summary>
    Task<(List<dynamic> data, int total)> GetFilteredAsync(
        int? matchId,
        int? playerId = null,
        int? teamId = null,
        string? position = null,
        string sort = "fantasy_points",
        string order = "desc",
        int limit = 50,
        int page = 1);

    /// <summary>
    /// Get player statistics filtered by gameweek (through match_id)
    /// </summary>
    Task<(List<dynamic> data, int total)> GetStatsByGameweekAsync(
        int gameweekId,
        int? playerId = null,
        int? teamId = null,
        string? position = null,
        string sort = "fantasy_points",
        string order = "desc",
        int limit = 50,
        int page = 1);

    /// <summary>
    /// Import player statistics records
    /// </summary>
    Task ImportAsync(List<dynamic> stats);
}
