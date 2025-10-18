namespace FantasyCoachAI.Application.DTOs;

/// <summary>
/// DTO for importing player statistics from CSV
/// </summary>
public class PlayerStatsImportDto
{
    public int PlayerId { get; set; }
    public int? MatchId { get; set; }
    public int FantasyPoints { get; set; }
    public int MinutesPlayed { get; set; }
    public int Goals { get; set; } = 0;
    public int Assists { get; set; } = 0;
    public int YellowCards { get; set; } = 0;
    public int RedCards { get; set; } = 0;
    public int Saves { get; set; } = 0;
    public int PenaltiesSaved { get; set; } = 0;
    public int PenaltiesWon { get; set; } = 0;
    public int PenaltiesScored { get; set; } = 0;
    public int PenaltiesCaused { get; set; } = 0;
    public int PenaltiesMissed { get; set; } = 0;
    public int LottoAssists { get; set; } = 0;
    public int OwnGoals { get; set; } = 0;
    public bool InTeamOfWeek { get; set; } = false;
    public decimal Price { get; set; }
    public bool PredictedStart { get; set; } = false;
    public string HealthStatus { get; set; } = "Pewny";
}

/// <summary>
/// Response DTO for stats import
/// </summary>
public class PlayerStatsImportResponseDto
{
    public bool Success { get; set; }
    public int ImportedCount { get; set; }
    public int SkippedCount { get; set; }
    public List<string> Errors { get; set; } = new();
}
