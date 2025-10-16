using FantasyCoachAI.Application.DTOs;
using FantasyCoachAI.Domain.Entities;

namespace FantasyCoachAI.Application.Interfaces
{
    public interface ITeamService
    {
        Task<List<TeamDto>> GetTeamsAsync(TeamFilterDto? filter = null);
        Task<TeamDto?> GetByIdAsync(int id);
        Task<TeamDto> CreateAsync(CreateTeamCommand command);
        Task<TeamDto> UpdateAsync(UpdateTeamCommand command);
    }
}
