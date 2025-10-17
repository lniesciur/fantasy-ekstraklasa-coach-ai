using FantasyCoachAI.Domain.Enums;

namespace FantasyCoachAI.Domain.Entities
{
    public class Gameweek
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        // Navigation properties
        public List<Match>? Matches { get; set; }

        // Domain logic - calculate status based on dates
        public GameweekStatus GetStatus()
        {
            var now = DateTime.UtcNow.Date;
            
            if (now < StartDate.Date)
                return GameweekStatus.Upcoming;
            
            if (now >= StartDate.Date && now <= EndDate.Date)
                return GameweekStatus.Current;
            
            return GameweekStatus.Completed;
        }

        // Domain validation
        public bool IsValidDateRange() => StartDate.Date < EndDate.Date;
        
        public bool HasStarted() => DateTime.UtcNow.Date >= StartDate.Date;
        
        public bool IsActive() => GetStatus() == GameweekStatus.Current;
    }
}
