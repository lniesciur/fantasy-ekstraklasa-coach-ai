using FantasyCoachAI.Domain.Enums;

namespace FantasyCoachAI.Application.DTOs
{
    public class UpdateMatchCommand
    {
        public int Id { get; set; }
        public int? GameweekId { get; set; }
        public int? HomeTeamId { get; set; }
        public int? AwayTeamId { get; set; }
        public DateTime? MatchDate { get; set; }
        public MatchStatus? Status { get; set; }
        public int? HomeScore { get; set; }
        public int? AwayScore { get; set; }
        public string? RescheduleReason { get; set; }
    }
}
