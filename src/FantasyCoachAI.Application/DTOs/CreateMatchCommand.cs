using FantasyCoachAI.Domain.Enums;

namespace FantasyCoachAI.Application.DTOs
{
    public class CreateMatchCommand
    {
        public int GameweekId { get; set; }
        public int HomeTeamId { get; set; }
        public int AwayTeamId { get; set; }
        public DateTime MatchDate { get; set; }
        public MatchStatus Status { get; set; } = MatchStatus.Scheduled;
    }
}
