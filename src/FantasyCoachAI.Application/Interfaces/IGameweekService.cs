using FantasyCoachAI.Application.DTOs;

namespace FantasyCoachAI.Application.Interfaces
{
    public interface IGameweekService
    {
        Task<List<GameweekDto>> GetGameweeksAsync(GameweekFilterDto? filter = null);
        Task<GameweekDto?> GetByIdAsync(int id);
        Task<GameweekDto?> GetByIdWithMatchesAsync(int id);
        Task<GameweekDto> CreateAsync(CreateGameweekCommand command);
        Task<GameweekDto> UpdateAsync(UpdateGameweekCommand command);
        Task<GameweekDto?> GetCurrentGameweekAsync();
        Task<GameweekDto?> GetGameweekByIdAsync(int id);
        Task<GameweekDto> CreateGameweekAsync(CreateGameweekCommand command);
        Task<GameweekDto> UpdateGameweekAsync(UpdateGameweekCommand command);
    }
}
