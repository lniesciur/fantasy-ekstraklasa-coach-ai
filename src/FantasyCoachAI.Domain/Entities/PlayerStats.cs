using FantasyCoachAI.Domain.Enums;

namespace FantasyCoachAI.Domain.Entities
{
    public class PlayerStats
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public int? MatchId { get; set; }
        public int? GameweekId { get; set; }
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
        public string? HealthStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        public Player? Player { get; set; }
        public Match? Match { get; set; }
        public Gameweek? Gameweek { get; set; }

        // Domain logic
        public bool IsStarter => MinutesPlayed > 0;
        public bool IsSubstitute => MinutesPlayed > 0 && !PredictedStart;
        public bool PlayedFullMatch => MinutesPlayed >= 90;
        public bool HasCleanSheet => Saves > 0 && Goals == 0; // Simplified - would need position context
        public int TotalPointsFromCards => YellowCards * (-1) + RedCards * (-3);
        public bool IsManOfTheMatch => InTeamOfWeek;

        public int CalculateFantasyPoints()
        {
            // Basic fantasy points calculation
            int points = Goals * 4 + Assists * 3 + Saves + PenaltiesSaved * 5 + PenaltiesScored * 5;
            points += TotalPointsFromCards;
            return points;
        }
    }
}
