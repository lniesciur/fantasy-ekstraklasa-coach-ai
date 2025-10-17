using FantasyCoachAI.Application.DTOs;

namespace FantasyCoachAI.Application.Interfaces
{
    public interface IGameweekService
    {
        Task<List<GameweekDto>> GetGameweeksAsync(GameweekFilterDto? filter = null);
        Task<GameweekDto?> GetByIdAsync(int id);
        Task<GameweekDto> CreateAsync(CreateGameweekCommand command);
        Task DeleteAsync(int id);
    }
}
