using FantasyCoachAI.Domain.Entities;
using FantasyCoachAI.Domain.Enums;

namespace FantasyCoachAI.Domain.Interfaces
{
    public interface IGameweekRepository
    {
        Task<List<Gameweek>> GetAllAsync();
        Task<Gameweek?> GetByIdAsync(int id);
        Task<Gameweek?> GetCurrentAsync();
        Task<Gameweek> CreateAsync(Gameweek gameweek);
        Task UpdateAsync(Gameweek gameweek);
        Task DeleteAsync(int id);
        Task<List<Gameweek>> GetFilteredAsync(
            GameweekStatus? status = null,
            string? sortBy = "number",
            bool ascending = true);
        Task<List<Gameweek>> GetAllWithMatchesAsync();
        Task<List<Gameweek>> GetFilteredWithMatchesAsync(
            GameweekStatus? status = null,
            string? sortBy = "number",
            bool ascending = true);
    }
}
