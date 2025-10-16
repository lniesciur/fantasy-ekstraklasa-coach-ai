using FantasyCoachAI.Domain.Enums;

namespace FantasyCoachAI.Domain.Entities
{
    public class Match
    {
        public int Id { get; set; }
        public int GameweekId { get; set; }
        public int HomeTeamId { get; set; }
        public int AwayTeamId { get; set; }
        public DateTime MatchDate { get; set; }
        public MatchStatus Status { get; set; }
        public int? HomeScore { get; set; }
        public int? AwayScore { get; set; }
        public string? RescheduleReason { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        public Gameweek? Gameweek { get; set; }
        public Team? HomeTeam { get; set; }
        public Team? AwayTeam { get; set; }

        // Domain logic
        public bool IsFinished() => Status == MatchStatus.Finished;
        public bool IsPostponed() => Status == MatchStatus.Postponed;
        public bool IsCancelled() => Status == MatchStatus.Cancelled;
        public bool HasResult() => HomeScore.HasValue && AwayScore.HasValue;
        
        public bool IsValidTeams() => HomeTeamId != AwayTeamId;
        
        public bool CanUpdateScore() => Status == MatchStatus.Live || Status == MatchStatus.Finished;
        
        public string GetScoreDisplay() => HasResult() ? $"{HomeScore} - {AwayScore}" : "vs";
    }
}
