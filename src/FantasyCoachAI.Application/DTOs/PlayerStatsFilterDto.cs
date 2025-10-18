namespace FantasyCoachAI.Application.DTOs;

/// <summary>
/// Filter parameters for getting player statistics
/// </summary>
public class PlayerStatsFilterDto
{
    /// <summary>
    /// Optional: Match ID to filter statistics (if null, returns all stats)
    /// </summary>
    public int? MatchId { get; set; }

    /// <summary>
    /// Optional: Filter by specific player ID
    /// </summary>
    public int? PlayerId { get; set; }

    /// <summary>
    /// Optional: Filter by team ID
    /// </summary>
    public int? TeamId { get; set; }

    /// <summary>
    /// Optional: Filter by position (GK, DEF, MID, FWD)
    /// </summary>
    public string? Position { get; set; }

    /// <summary>
    /// Sort field: fantasy_points, form, price (default: fantasy_points)
    /// </summary>
    public string Sort { get; set; } = "fantasy_points";

    /// <summary>
    /// Sort order: asc, desc (default: desc)
    /// </summary>
    public string Order { get; set; } = "desc";

    /// <summary>
    /// Results per page (default: 50, max: 100)
    /// </summary>
    public int Limit { get; set; } = 50;

    /// <summary>
    /// Page number (default: 1)
    /// </summary>
    public int Page { get; set; } = 1;
}
