using FantasyCoachAI.Application.DTOs;
using FantasyCoachAI.Application.Interfaces;
using FantasyCoachAI.Domain.Entities;
using FantasyCoachAI.Domain.Interfaces;
using FantasyCoachAI.Domain.Exceptions;

namespace FantasyCoachAI.Application.Services
{
    public class GameweekService : IGameweekService
    {
        private readonly IGameweekRepository _gameweekRepository;
        private readonly IMatchRepository _matchRepository;

        public GameweekService(IGameweekRepository gameweekRepository, IMatchRepository matchRepository)
        {
            _gameweekRepository = gameweekRepository;
            _matchRepository = matchRepository;
        }

        public async Task<List<GameweekDto>> GetGameweeksAsync(GameweekFilterDto? filter = null)
        {
            var gameweeks = await _gameweekRepository.GetFilteredAsync(
                    status: filter?.Status,
                    sortBy: filter?.Sort,
                    ascending: filter?.IsAscending ?? true);

            return gameweeks.Select(gameweek =>
            {
                var dto = MapToDto(gameweek);
                if (gameweek.Matches != null && gameweek.Matches.Any())
                {
                    var matchDtos = gameweek.Matches.Select(MapMatchToDto).ToList();
                    dto.MatchSummaries = matchDtos.Select(CreateMatchSummary).ToList();
                }
                return dto;
            }).ToList();
        }

        public async Task<GameweekDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Id must be greater than zero", nameof(id));

            var gameweek = await _gameweekRepository.GetByIdAsync(id);
            var dto = gameweek != null ? MapToDto(gameweek) : null;
            if (dto == null)
                return null;

            dto.MatchSummaries = gameweek?.Matches != null
                ? gameweek.Matches.Select(MapMatchToDto).Select(CreateMatchSummary).ToList()
                : new List<MatchSummaryDto>();

            return dto;
        }

        // Alias for GetByIdAsync to maintain compatibility with tests
        public async Task<GameweekDto?> GetGameweekByIdAsync(int id)
        {
            return await GetByIdAsync(id);
        }

        public async Task<GameweekDto> CreateAsync(CreateGameweekCommand command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            ValidateGameweekCommand(command.Number, command.StartDate, command.EndDate);

            // Check if gameweek number already exists
            var existingGameweek = await _gameweekRepository.GetByNumberAsync(command.Number);
            if (existingGameweek != null)
            {
                throw new InvalidOperationException($"Gameweek with number {command.Number} already exists");
            }

            var gameweek = new Gameweek
            {
                Number = command.Number,
                StartDate = command.StartDate,
                EndDate = command.EndDate
            };

            var createdGameweek = await _gameweekRepository.CreateAsync(gameweek);
            return MapToDto(createdGameweek);
        }

        // Alias for CreateAsync to maintain compatibility with tests
        public async Task<GameweekDto> CreateGameweekAsync(CreateGameweekCommand command)
        {
            return await CreateAsync(command);
        }

        public async Task DeleteAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Id must be greater than zero", nameof(id));

            // Check if gameweek exists
            var existingGameweek = await _gameweekRepository.GetByIdAsync(id);
            if (existingGameweek == null)
            {
                throw new NotFoundException($"Gameweek with ID {id} not found");
            }

            // Check if gameweek has associated matches
            var matches = await _matchRepository.GetByGameweekIdAsync(id);
            if (matches.Any())
            {
                throw new InvalidOperationException($"Cannot delete gameweek with ID {id} because it has associated matches. Please delete matches first.");
            }

            await _gameweekRepository.DeleteAsync(id);
        }

        private static void ValidateGameweekCommand(int number, DateTime startDate, DateTime endDate)
        {
            if (number <= 0)
                throw new ArgumentException("Gameweek number must be greater than zero");

            if (startDate.Date >= endDate.Date)
                throw new ArgumentException("Start date must be before end date");

            if (startDate.Date < DateTime.UtcNow.Date.AddYears(-1))
                throw new ArgumentException("Start date cannot be more than 1 year in the past");

            if (endDate.Date > DateTime.UtcNow.Date.AddYears(2))
                throw new ArgumentException("End date cannot be more than 2 years in the future");
        }

        private static GameweekDto MapToDto(Gameweek gameweek)
        {
            return new GameweekDto
            {
                Id = gameweek.Id,
                Number = gameweek.Number,
                StartDate = gameweek.StartDate,
                EndDate = gameweek.EndDate,
                Status = gameweek.GetStatus(),
                MatchSummaries = null // Will be populated by calling methods
            };
        }

        // Helper for creating match summary string
        private static MatchSummaryDto CreateMatchSummary(MatchDto match)
        {
            var homeTeamName = match.HomeTeam?.Name ?? "Unknown";
            var awayTeamName = match.AwayTeam?.Name ?? "Unknown";

            return new MatchSummaryDto
            {
                MatchId = match.Id,
                HomeTeamName = homeTeamName,
                HomeTeamScore = match.HomeScore,
                AwayTeamName = awayTeamName,
                AwayTeamScore = match.AwayScore,
                MatchDate = match.MatchDate,
                Status = match.Status.ToString()
            };
        }

        // Helper for mapping matches if needed
        private static MatchDto MapMatchToDto(Match match)
        {
            return new MatchDto
            {
                Id = match.Id,
                Gameweek = match.Gameweek != null ? new GameweekDto { Id = match.Gameweek.Id, Number = match.Gameweek.Number } : null,
                HomeTeam = match.HomeTeam != null ? new TeamDto { Id = match.HomeTeam.Id, Name = match.HomeTeam.Name, ShortCode = match.HomeTeam.ShortCode } : new TeamDto { Id = match.HomeTeamId },
                AwayTeam = match.AwayTeam != null ? new TeamDto { Id = match.AwayTeam.Id, Name = match.AwayTeam.Name, ShortCode = match.AwayTeam.ShortCode } : new TeamDto { Id = match.AwayTeamId },
                MatchDate = match.MatchDate,
                Status = match.Status,
                HomeScore = match.HomeScore,
                AwayScore = match.AwayScore,
                RescheduleReason = match.RescheduleReason,
                CreatedAt = match.CreatedAt,
                UpdatedAt = match.UpdatedAt
            };
        }
    }
}
