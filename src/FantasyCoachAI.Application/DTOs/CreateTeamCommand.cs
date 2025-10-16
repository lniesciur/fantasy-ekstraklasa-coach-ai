namespace FantasyCoachAI.Application.DTOs
{
    public class CreateTeamCommand
    {
        public string Name { get; set; } = string.Empty;
        public string ShortCode { get; set; } = string.Empty;
        public string? CrestUrl { get; set; }
        public bool? IsActive { get; set; }
    }
}
