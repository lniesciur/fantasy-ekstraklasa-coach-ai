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
        public int MatchesCount { get; set; }
        public List<MatchDto>? Matches { get; set; }
    }
}
