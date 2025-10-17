using FantasyCoachAI.Domain.Enums;

namespace FantasyCoachAI.Application.DTOs
{
    public class MatchDto
    {
        public int Id { get; set; }
        public GameweekDto? Gameweek { get; set; }
        public TeamDto? HomeTeam { get; set; }
        public TeamDto? AwayTeam { get; set; }
        public DateTime MatchDate { get; set; }
        public MatchStatus Status { get; set; }
        public int? HomeScore { get; set; }
        public int? AwayScore { get; set; }
        public string? RescheduleReason { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
