using FantasyCoachAI.Domain.Enums;

namespace FantasyCoachAI.Application.DTOs
{
    public class GameweekDto
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public GameweekStatus Status { get; set; }
        public List<MatchSummaryDto> MatchSummaries { get; set; } = new List<MatchSummaryDto>();
    }

    public class MatchSummaryDto
    {
        public int MatchId { get; set; }
        public required string HomeTeamName { get; set; }
        public int? HomeTeamScore { get; set; }
        public required string AwayTeamName { get; set; }
        public int? AwayTeamScore { get; set; }
        public DateTime MatchDate { get; set; }
        public required string Status { get; set; }
    }
}
