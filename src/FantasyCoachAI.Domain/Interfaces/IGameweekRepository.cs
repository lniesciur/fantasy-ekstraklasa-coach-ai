using FantasyCoachAI.Domain.Entities;
using FantasyCoachAI.Domain.Enums;

namespace FantasyCoachAI.Domain.Interfaces
{
    public interface IGameweekRepository
    {
        Task<Gameweek?> GetByIdAsync(int id);
        Task<Gameweek?> GetByNumberAsync(int number);
        Task<Gameweek> CreateAsync(Gameweek gameweek);
        Task DeleteAsync(int id);
        Task<List<Gameweek>> GetFilteredAsync(
            GameweekStatus? status = null,
            string? sortBy = "number",
            bool ascending = true);
    }
}
