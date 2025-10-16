using FantasyCoachAI.Domain.Entities;

namespace FantasyCoachAI.Domain.Interfaces
{
    public interface ITeamRepository
    {
        Task<List<Team>> GetAllAsync();
        Task<Team?> GetByIdAsync(int id);
        Task<Team> CreateAsync(Team team);
        Task UpdateAsync(Team team);    
        Task<List<Team>> GetFilteredAsync(bool? isActive = null, string? shortCode = null);
    }
}
