using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace FantasyCoachAI.Infrastructure.Persistence.SupabaseModels;

/// <summary>
/// Supabase model for player_stats table
/// </summary>
[Table("player_stats")]
public class PlayerStatsDbModel : BaseModel
{
    [PrimaryKey("id", false)]
    public int Id { get; set; }

    [Column("player_id")]
    public int PlayerId { get; set; }


    [Column("match_id")]
    public int? MatchId { get; set; }

    [Column("fantasy_points")]
    public int FantasyPoints { get; set; }

    [Column("minutes_played")]
    public int MinutesPlayed { get; set; }

    [Column("goals")]
    public int Goals { get; set; }

    [Column("assists")]
    public int Assists { get; set; }

    [Column("yellow_cards")]
    public int YellowCards { get; set; }

    [Column("red_cards")]
    public int RedCards { get; set; }

    [Column("saves")]
    public int Saves { get; set; }

    [Column("penalties_saved")]
    public int PenaltiesSaved { get; set; }

    [Column("penalties_won")]
    public int PenaltiesWon { get; set; }

    [Column("penalties_scored")]
    public int PenaltiesScored { get; set; }

    [Column("penalties_caused")]
    public int PenaltiesCaused { get; set; }

    [Column("penalties_missed")]
    public int PenaltiesMissed { get; set; }

    [Column("lotto_assists")]
    public int LottoAssists { get; set; }

    [Column("own_goals")]
    public int OwnGoals { get; set; }

    [Column("in_team_of_week")]
    public bool InTeamOfWeek { get; set; }

    [Column("price")]
    public decimal Price { get; set; }

    [Column("predicted_start")]
    public bool PredictedStart { get; set; }

    [Column("health_status")]
    public string HealthStatus { get; set; } = "Pewny";

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }

    // Navigation properties for joins
    [Column("player")]
    public dynamic? Player { get; set; }
    
    [Column("match")]
    public dynamic? Match { get; set; }
}
