namespace FantasyCoachAI.Application.DTOs
{
    public class LineupDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int GameweekId { get; set; }
        public string Formation { get; set; } = "4-3-3";
        public decimal TotalCost { get; set; }
        public decimal RemainingBudget { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<LineupPlayerDto> Players { get; set; } = new();
    }

    public class LineupResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int GameweekId { get; set; }
        public string Formation { get; set; } = "4-3-3";
        public decimal TotalCost { get; set; }
        public decimal RemainingBudget { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public int TotalPlayers { get; set; }
        public int GoalkeeperCount { get; set; }
        public int DefenderCount { get; set; }
        public int MidfielderCount { get; set; }
        public int ForwardCount { get; set; }
    }

    public class LineupPlayerDto
    {
        public int PlayerId { get; set; }
        public string PlayerName { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int TeamId { get; set; }
        public string TeamName { get; set; } = string.Empty;
        public bool IsCaptain { get; set; }
        public bool IsViceCaptain { get; set; }
    }

    public class CreateLineupCommand
    {
        public string Name { get; set; } = string.Empty;
        public int GameweekId { get; set; }
        public string Formation { get; set; } = "4-3-3";
        public List<int> PlayerIds { get; set; } = new();
        public int? CaptainPlayerId { get; set; }
        public int? ViceCaptainPlayerId { get; set; }
        public bool SetAsActive { get; set; }
    }

    public class UpdateLineupCommand
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Formation { get; set; }
        public List<int>? PlayerIds { get; set; }
        public int? CaptainPlayerId { get; set; }
        public int? ViceCaptainPlayerId { get; set; }
    }

    public class LineupFilterDto
    {
        public int? GameweekId { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedFromDate { get; set; }
        public DateTime? CreatedToDate { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
