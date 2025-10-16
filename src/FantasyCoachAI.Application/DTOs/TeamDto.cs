namespace FantasyCoachAI.Application.DTOs
{
    public class TeamDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ShortCode { get; set; }
        public string? CrestUrl { get; set; }
        public bool IsActive { get; set; }
    }
}
