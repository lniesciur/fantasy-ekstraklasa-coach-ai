using FantasyCoachAI.Application.DTOs;

namespace FantasyCoachAI.Application.Interfaces
{
    public interface ILineupService
    {
        Task<LineupDto?> GetLineupByIdAsync(int id);
        Task<LineupDto?> GetActiveLineupAsync();
        Task<List<LineupResponseDto>> GetRecentLineupsAsync(int count = 3);
        Task<PaginationDto<LineupResponseDto>> GetLineupsAsync(LineupFilterDto filter);
        Task<LineupDto> CreateLineupAsync(CreateLineupCommand command);
        Task<LineupDto> UpdateLineupAsync(UpdateLineupCommand command);
        Task<bool> DeleteLineupAsync(int id);
        Task<bool> SetActiveLineupAsync(int id);
    }
}
