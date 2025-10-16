using FantasyCoachAI.Domain.Enums;

namespace FantasyCoachAI.Application.DTOs
{
    public class MatchFilterDto
    {
        public int? GameweekId { get; set; }
        public int? TeamId { get; set; }
        public MatchStatus? Status { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string Sort { get; set; } = "match_date";
        public string Order { get; set; } = "asc";
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 50;
    }
}
