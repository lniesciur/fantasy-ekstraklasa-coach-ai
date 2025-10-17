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
        private readonly IMatchRepository _matchRepository; // Inject for matches

        public GameweekService(IGameweekRepository gameweekRepository, IMatchRepository matchRepository)
        {
            _gameweekRepository = gameweekRepository;
            _matchRepository = matchRepository;
        }

        public async Task<List<GameweekDto>> GetGameweeksAsync(GameweekFilterDto? filter = null)
        {
            var gameweeks = filter != null
                ? await _gameweekRepository.GetFilteredWithMatchesAsync(
                    status: filter.Status,
                    sortBy: filter.Sort,
                    ascending: filter.IsAscending)
                : await _gameweekRepository.GetAllWithMatchesAsync();

            return gameweeks.Select(gameweek =>
            {
                var dto = MapToDto(gameweek);
                if (gameweek.Matches != null && gameweek.Matches.Any())
                {
                    var matchDtos = gameweek.Matches.Select(MapMatchToDto).ToList();
                    dto.MatchesCount = matchDtos.Count;
                    dto.MatchSummaries = matchDtos.Select(CreateMatchSummary).ToList();
                }
                else
                {
                    dto.MatchesCount = 0;
                    dto.MatchSummaries = new List<string>();
                }
                return dto;
            }).ToList();
        }

        public async Task<GameweekDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Id must be greater than zero", nameof(id));

            var gameweek = await _gameweekRepository.GetByIdAsync(id);
            return gameweek != null ? MapToDto(gameweek) : null;
        }

        // Alias for GetByIdAsync to maintain compatibility with tests
        public async Task<GameweekDto?> GetGameweekByIdAsync(int id)
        {
            return await GetByIdAsync(id);
        }

        public async Task<GameweekDto?> GetByIdWithMatchesAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Id must be greater than zero", nameof(id));

            var gameweek = await _gameweekRepository.GetByIdAsync(id);
            if (gameweek == null)
                return null;

            // Fetch matches for this gameweek
            var matches = await _matchRepository.GetByGameweekIdAsync(id);
            var matchDtos = matches.Select(MapMatchToDto).ToList();

            var dto = MapToDto(gameweek);
            dto.MatchesCount = matchDtos.Count();
            dto.MatchSummaries = matchDtos.Select(CreateMatchSummary).ToList();

            return dto;
        }

        public async Task<GameweekDto> CreateAsync(CreateGameweekCommand command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            ValidateGameweekCommand(command.Number, command.StartDate, command.EndDate);

            // Check if gameweek number already exists
            var existingGameweeks = await _gameweekRepository.GetAllAsync();
            if (existingGameweeks.Any(g => g.Number == command.Number))
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

        public async Task<GameweekDto> UpdateAsync(UpdateGameweekCommand command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            if (command.Id <= 0)
                throw new ArgumentException("Id must be greater than zero", nameof(command));

            ValidateGameweekCommand(command.Number, command.StartDate, command.EndDate);

            // Check if gameweek exists
            var existingGameweek = await _gameweekRepository.GetByIdAsync(command.Id);
            if (existingGameweek == null)
            {
                throw new NotFoundException($"Gameweek with ID {command.Id} not found");
            }

            // Check if gameweek number already exists for a different gameweek
            if (existingGameweek.Number != command.Number)
            {
                var allGameweeks = await _gameweekRepository.GetAllAsync();
                if (allGameweeks.Any(g => g.Number == command.Number && g.Id != command.Id))
                {
                    throw new InvalidOperationException($"Gameweek with number {command.Number} already exists");
                }
            }

            // Update the gameweek
            existingGameweek.Number = command.Number;
            existingGameweek.StartDate = command.StartDate;
            existingGameweek.EndDate = command.EndDate;

            await _gameweekRepository.UpdateAsync(existingGameweek);
            return MapToDto(existingGameweek);
        }

        // Alias for UpdateAsync to maintain compatibility with tests
        public async Task<GameweekDto> UpdateGameweekAsync(UpdateGameweekCommand command)
        {
            return await UpdateAsync(command);
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
                MatchesCount = 0, // Will be populated by calling methods
                MatchSummaries = null // Will be populated by calling methods
            };
        }

        // Helper for creating match summary string
        private static string CreateMatchSummary(MatchDto match)
        {
            var homeTeamName = match.HomeTeam?.Name ?? "Unknown";
            var awayTeamName = match.AwayTeam?.Name ?? "Unknown";

            string scoreDisplay;
            if (match.HomeScore.HasValue && match.AwayScore.HasValue)
            {
                scoreDisplay = $"{match.HomeScore}-{match.AwayScore}";
            }
            else
            {
                scoreDisplay = "vs";
            }

            return $"{homeTeamName} - {awayTeamName} {scoreDisplay}";
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
