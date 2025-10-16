namespace FantasyCoachAI.Application.DTOs
{
    public class MatchResponseDto
    {
        public List<MatchDto> Data { get; set; } = new();
        public PaginationDto Pagination { get; set; } = new();
    }
}
