using FantasyCoachAI.Application.DTOs;

namespace FantasyCoachAI.Application.Interfaces
{
    public interface IMatchService
    {
        Task<MatchResponseDto> GetMatchesAsync(MatchFilterDto filter);
        Task<MatchResponseDto> GetAllAsync(MatchFilterDto filter);
        Task<MatchDto?> GetByIdAsync(int id);
        Task<MatchDto> CreateAsync(CreateMatchCommand command);
        Task<MatchDto> UpdateAsync(UpdateMatchCommand command);
    }
}
