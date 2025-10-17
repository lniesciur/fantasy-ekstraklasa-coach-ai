using FantasyCoachAI.Application.DTOs;
using FantasyCoachAI.Application.Interfaces;
using FantasyCoachAI.Domain.Entities;
using FantasyCoachAI.Domain.Interfaces;
using FantasyCoachAI.Domain.Exceptions;
using FantasyCoachAI.Application.Validators; // Assuming validators exist

namespace FantasyCoachAI.Application.Services
{
    public class TeamService : ITeamService
    {
        private readonly ITeamRepository _teamRepository;

        public TeamService(ITeamRepository teamRepository)
        {
            _teamRepository = teamRepository;
        }

        public async Task<List<TeamDto>> GetTeamsAsync(TeamFilterDto? filter = null)
        {
            IEnumerable<Team> teams;

            if (filter == null)
            {
                teams = await _teamRepository.GetAllAsync();
            }
            else
            {
                teams = await _teamRepository.GetFilteredAsync(
                    isActive: filter.IsActive,
                    shortCode: filter.ShortCode
                );
            }

            // Apply sorting
            teams = ApplySorting(teams, filter?.Sort, filter?.Order);

            return teams.Select(MapToDto).ToList();
        }

        public async Task<TeamDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("ID must be greater than 0", nameof(id));
            }

            var team = await _teamRepository.GetByIdAsync(id);
            return team != null ? MapToDto(team) : null;
        }

        public async Task<TeamDto> CreateAsync(CreateTeamCommand command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            if (string.IsNullOrWhiteSpace(command.Name))
                throw new ArgumentException("Name is required");

            if (string.IsNullOrWhiteSpace(command.ShortCode))
                throw new ArgumentException("ShortCode is required");

            // Check uniqueness
            var existingTeams = await _teamRepository.GetAllAsync();
            if (existingTeams.Any(t => t.Name.Equals(command.Name, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException($"A team named '{command.Name}' already exists");

            if (existingTeams.Any(t => t.ShortCode == command.ShortCode))
                throw new InvalidOperationException($"Short code '{command.ShortCode}' already exists");

            var team = new Team
            {
                Name = command.Name,
                ShortCode = command.ShortCode,
                CrestUrl = command.CrestUrl,
                IsActive = command.IsActive ?? true
            };

            var createdTeam = await _teamRepository.CreateAsync(team);
            return MapToDto(createdTeam);
        }

        public async Task<TeamDto> UpdateAsync(UpdateTeamCommand command)
        {
            var existingTeam = await _teamRepository.GetByIdAsync(command.Id);
            if (existingTeam == null)
                throw new NotFoundException($"Team with ID {command.Id} not found");

            existingTeam.IsActive = command.IsActive;

            await _teamRepository.UpdateAsync(existingTeam);
            return MapToDto(existingTeam);
        }

        private static IEnumerable<Team> ApplySorting(IEnumerable<Team> teams, string? sort, string? order)
        {
            if (string.IsNullOrWhiteSpace(sort))
                return teams;

            var isDescending = "desc".Equals(order, StringComparison.OrdinalIgnoreCase);

            return sort.ToLower() switch
            {
                "name" => isDescending
                    ? teams.OrderByDescending(t => t.Name)
                    : teams.OrderBy(t => t.Name),
                "shortcode" => isDescending
                    ? teams.OrderByDescending(t => t.ShortCode)
                    : teams.OrderBy(t => t.ShortCode),
                _ => teams
            };
        }

        private static TeamDto MapToDto(Team team)
        {
            return new TeamDto
            {
                Id = team.Id,
                Name = team.Name,
                ShortCode = team.ShortCode,
                CrestUrl = team.CrestUrl,
                IsActive = team.IsActive
                // Add other fields like league_position, form, stats if needed from plan
            };
        }
    }
}
