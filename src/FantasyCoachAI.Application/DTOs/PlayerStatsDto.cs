namespace FantasyCoachAI.Application.DTOs;

/// <summary>
/// DTO for player statistics in a gameweek
/// </summary>
public class PlayerStatsDto
{
    public int Id { get; set; }
    public PlayerStatsPlayerDto Player { get; set; } = null!;
    public GameweekStatsDto? Gameweek { get; set; }
    public int FantasyPoints { get; set; }
    public int MinutesPlayed { get; set; }
    public int Goals { get; set; }
    public int Assists { get; set; }
    public int YellowCards { get; set; }
    public int RedCards { get; set; }
    public int Saves { get; set; }
    public int PenaltiesSaved { get; set; }
    public int PenaltiesWon { get; set; }
    public int PenaltiesScored { get; set; }
    public int PenaltiesCaused { get; set; }
    public int PenaltiesMissed { get; set; }
    public int LottoAssists { get; set; }
    public int OwnGoals { get; set; }
    public bool InTeamOfWeek { get; set; }
    public decimal Price { get; set; }
    public bool PredictedStart { get; set; }
    public string HealthStatus { get; set; } = "Pewny";
}

public class PlayerStatsPlayerDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Position { get; set; } = null!;
}

public class GameweekStatsDto
{
    public int Id { get; set; }
    public int Number { get; set; }
}
