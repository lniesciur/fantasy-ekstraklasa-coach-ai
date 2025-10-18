using FantasyCoachAI.Domain.Enums;

namespace FantasyCoachAI.Domain.Entities
{
    public class Player
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int TeamId { get; set; }
        public Position Position { get; set; }
        public decimal Price { get; set; }
        public HealthStatus HealthStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        public Team? Team { get; set; }
        public ICollection<PlayerStats>? PlayerStats { get; set; }

        // Domain logic
        public bool IsFit => HealthStatus == HealthStatus.Available;
        public bool IsInjured => HealthStatus == HealthStatus.Injured;
        public bool IsDoubtful => HealthStatus == HealthStatus.Doubtful;
        public bool IsGoalkeeper => Position == Position.GK;
        public bool IsDefender => Position == Position.DEF;
        public bool IsMidfielder => Position == Position.MID;
        public bool IsForward => Position == Position.FWD;

        public decimal GetPriceInMillions() => Price / 1000000;
        public bool CanPlayInPosition(Position position) => Position == position;
    }
}
