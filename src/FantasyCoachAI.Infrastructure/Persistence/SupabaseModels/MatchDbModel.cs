using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace FantasyCoachAI.Infrastructure.Persistence.SupabaseModels
{
    [Table("matches")]
    public class MatchDbModel : BaseModel
    {
        [PrimaryKey("id", true)]
        [Column("id")]
        public int Id { get; set; }

        [Column("gameweek_id")]
        public int GameweekId { get; set; }

        [Column("home_team_id")]
        public int HomeTeamId { get; set; }

        [Column("away_team_id")]
        public int AwayTeamId { get; set; }

        [Column("match_date")]
        public DateTime MatchDate { get; set; }

        [Column("status")]
        public string Status { get; set; } = "scheduled";

        [Column("home_score")]
        public int? HomeScore { get; set; }

        [Column("away_score")]
        public int? AwayScore { get; set; }

        [Column("reschedule_reason")]
        public string? RescheduleReason { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        // Navigation properties for joins
        [Column("gameweek")]
        public GameweekDbModel? Gameweek { get; set; }
        
        [Column("home_team")]
        public TeamDbModel? HomeTeam { get; set; }
        
        [Column("away_team")]
        public TeamDbModel? AwayTeam { get; set; }
    }
}
