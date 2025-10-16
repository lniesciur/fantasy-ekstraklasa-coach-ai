namespace FantasyCoachAI.Domain.Entities
{
    public class Team
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ShortCode { get; set; }
        public string? CrestUrl { get; set; }
        public bool IsActive { get; set; }

        // Domain logic
        public bool HasCrest() => !string.IsNullOrWhiteSpace(CrestUrl);
    }
}
