namespace FantasyCoachAI.Application.DTOs
{
    public class TeamFilterDto
    {
        public bool? IsActive { get; set; }
        public string? ShortCode { get; set; }
        public string? Sort { get; set; }
        public string? Order { get; set; }
    }
}
