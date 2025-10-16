using FantasyCoachAI.Domain.Enums;

namespace FantasyCoachAI.Application.DTOs
{
    public class GameweekFilterDto
    {
        public GameweekStatus? Status { get; set; }
        public string? Sort { get; set; } = "number";
        public string? Order { get; set; } = "asc";
        
        public bool IsAscending => string.Equals(Order, "asc", StringComparison.OrdinalIgnoreCase);
    }
}
