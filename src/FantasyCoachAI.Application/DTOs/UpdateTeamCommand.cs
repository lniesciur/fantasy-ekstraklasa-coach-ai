namespace FantasyCoachAI.Application.DTOs
{
    public class UpdateTeamCommand
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ShortCode { get; set; } = string.Empty;
        public string? CrestUrl { get; set; }
        public bool? IsActive { get; set; }
    }
}
