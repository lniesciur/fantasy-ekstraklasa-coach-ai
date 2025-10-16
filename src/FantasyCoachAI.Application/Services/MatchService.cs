using FantasyCoachAI.Application.DTOs;
using FantasyCoachAI.Application.Interfaces;
using FantasyCoachAI.Domain.Entities;
using FantasyCoachAI.Domain.Interfaces;
using FantasyCoachAI.Domain.Exceptions;

namespace FantasyCoachAI.Application.Services
{
    public class MatchService : IMatchService
    {
        private readonly IMatchRepository _matchRepository;
        private readonly IGameweekRepository _gameweekRepository;
        private readonly ITeamRepository _teamRepository;

        public MatchService(
            IMatchRepository matchRepository,
            IGameweekRepository gameweekRepository,
            ITeamRepository teamRepository)
        {
            _matchRepository = matchRepository;
            _gameweekRepository = gameweekRepository;
            _teamRepository = teamRepository;
        }

        // Alias for controller: GetAllAsync -> GetMatchesAsync
        public async Task<MatchResponseDto> GetAllAsync(MatchFilterDto filter)
        {
            return await GetMatchesAsync(filter);
        }

        public async Task<MatchResponseDto> GetMatchesAsync(MatchFilterDto filter)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));

            // Validate pagination parameters
            ValidatePaginationFilter(filter);

            var (matches, totalCount) = await _matchRepository.GetMatchesAsync(
                gameweekId: filter.GameweekId,
                teamId: filter.TeamId,
                status: filter.Status,
                dateFrom: filter.DateFrom,
                dateTo: filter.DateTo,
                sortBy: filter.Sort,
                ascending: filter.Order == "asc",
                page: filter.Page,
                limit: filter.Limit);

            var matchDtos = matches.Select(MapToDto).ToList();
            var pagination = PaginationDto.Create(filter.Page, filter.Limit, totalCount);

            return new MatchResponseDto
            {
                Data = matchDtos,
                Pagination = pagination
            };
        }

        public async Task<MatchDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Id must be greater than zero", nameof(id));

            var match = await _matchRepository.GetByIdAsync(id);
            return match != null ? MapToDto(match) : null;
        }

        public async Task<MatchDto> CreateAsync(CreateMatchCommand command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            await ValidateCreateMatchCommand(command);

            var match = new Match
            {
                GameweekId = command.GameweekId,
                HomeTeamId = command.HomeTeamId,
                AwayTeamId = command.AwayTeamId,
                MatchDate = command.MatchDate,
                Status = command.Status
            };

            var createdMatch = await _matchRepository.CreateAsync(match);
            return MapToDto(createdMatch);
        }

        public async Task<MatchDto> UpdateAsync(UpdateMatchCommand command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            if (command.Id <= 0)
                throw new ArgumentException("Id must be greater than zero", nameof(command.Id));

            // Check if match exists
            var existingMatch = await _matchRepository.GetByIdAsync(command.Id);
            if (existingMatch == null)
            {
                throw new NotFoundException($"Match with ID {command.Id} not found");
            }

            await ValidateUpdateMatchCommand(command, existingMatch);

            // Apply updates only for provided fields (partial update)
            if (command.GameweekId.HasValue)
                existingMatch.GameweekId = command.GameweekId.Value;

            if (command.HomeTeamId.HasValue)
                existingMatch.HomeTeamId = command.HomeTeamId.Value;

            if (command.AwayTeamId.HasValue)
                existingMatch.AwayTeamId = command.AwayTeamId.Value;

            if (command.MatchDate.HasValue)
                existingMatch.MatchDate = command.MatchDate.Value;

            if (command.Status.HasValue)
                existingMatch.Status = command.Status.Value;

            if (command.HomeScore.HasValue)
                existingMatch.HomeScore = command.HomeScore.Value;

            if (command.AwayScore.HasValue)
                existingMatch.AwayScore = command.AwayScore.Value;

            if (command.RescheduleReason != null)
                existingMatch.RescheduleReason = command.RescheduleReason;

            // Validate teams are not the same after update
            if (existingMatch.HomeTeamId == existingMatch.AwayTeamId)
            {
                throw new InvalidOperationException("Home team and away team cannot be the same");
            }

            await _matchRepository.UpdateAsync(existingMatch);
            return MapToDto(existingMatch);
        }

        private async Task ValidateCreateMatchCommand(CreateMatchCommand command)
        {
            // Validate basic fields
            if (command.GameweekId <= 0)
                throw new ArgumentException("GameweekId must be greater than zero");

            if (command.HomeTeamId <= 0)
                throw new ArgumentException("HomeTeamId must be greater than zero");

            if (command.AwayTeamId <= 0)
                throw new ArgumentException("AwayTeamId must be greater than zero");

            if (command.HomeTeamId == command.AwayTeamId)
                throw new InvalidOperationException("Home team and away team cannot be the same");

            // Check if gameweek exists
            var gameweek = await _gameweekRepository.GetByIdAsync(command.GameweekId);
            if (gameweek == null)
            {
                throw new NotFoundException($"Gameweek with ID {command.GameweekId} not found");
            }

            // Check if teams exist and are active
            var homeTeam = await _teamRepository.GetByIdAsync(command.HomeTeamId);
            if (homeTeam == null)
            {
                throw new NotFoundException($"Home team with ID {command.HomeTeamId} not found");
            }

            var awayTeam = await _teamRepository.GetByIdAsync(command.AwayTeamId);
            if (awayTeam == null)
            {
                throw new NotFoundException($"Away team with ID {command.AwayTeamId} not found");
            }

            if (!homeTeam.IsActive)
            {
                throw new InvalidOperationException($"Home team '{homeTeam.Name}' is not active");
            }

            if (!awayTeam.IsActive)
            {
                throw new InvalidOperationException($"Away team '{awayTeam.Name}' is not active");
            }

            // Validate match date is within gameweek range
            if (command.MatchDate.Date < gameweek.StartDate.Date || 
                command.MatchDate.Date > gameweek.EndDate.Date)
            {
                throw new InvalidOperationException(
                    $"Match date must be between {gameweek.StartDate:yyyy-MM-dd} and {gameweek.EndDate:yyyy-MM-dd}");
            }

            // Check if match between these teams already exists in this gameweek
            var matchExists = await _matchRepository.ExistsMatchBetweenTeamsInGameweekAsync(
                command.HomeTeamId, command.AwayTeamId, command.GameweekId);
            
            if (matchExists)
            {
                throw new InvalidOperationException(
                    $"A match between {homeTeam.Name} and {awayTeam.Name} already exists in gameweek {gameweek.Number}");
            }
        }

        private async Task ValidateUpdateMatchCommand(UpdateMatchCommand command, Match existingMatch)
        {
            // Validate gameweek if changing
            if (command.GameweekId.HasValue)
            {
                if (command.GameweekId.Value <= 0)
                    throw new ArgumentException("GameweekId must be greater than zero");

                var gameweek = await _gameweekRepository.GetByIdAsync(command.GameweekId.Value);
                if (gameweek == null)
                {
                    throw new NotFoundException($"Gameweek with ID {command.GameweekId.Value} not found");
                }

                // Validate match date within new gameweek range
                var matchDate = command.MatchDate ?? existingMatch.MatchDate;
                if (matchDate.Date < gameweek.StartDate.Date || matchDate.Date > gameweek.EndDate.Date)
                {
                    throw new InvalidOperationException(
                        $"Match date must be between {gameweek.StartDate:yyyy-MM-dd} and {gameweek.EndDate:yyyy-MM-dd}");
                }
            }

            // Validate teams if changing
            if (command.HomeTeamId.HasValue)
            {
                if (command.HomeTeamId.Value <= 0)
                    throw new ArgumentException("HomeTeamId must be greater than zero");

                var homeTeam = await _teamRepository.GetByIdAsync(command.HomeTeamId.Value);
                if (homeTeam == null)
                {
                    throw new NotFoundException($"Home team with ID {command.HomeTeamId.Value} not found");
                }

                if (!homeTeam.IsActive)
                {
                    throw new InvalidOperationException($"Home team '{homeTeam.Name}' is not active");
                }
            }

            if (command.AwayTeamId.HasValue)
            {
                if (command.AwayTeamId.Value <= 0)
                    throw new ArgumentException("AwayTeamId must be greater than zero");

                var awayTeam = await _teamRepository.GetByIdAsync(command.AwayTeamId.Value);
                if (awayTeam == null)
                {
                    throw new NotFoundException($"Away team with ID {command.AwayTeamId.Value} not found");
                }

                if (!awayTeam.IsActive)
                {
                    throw new InvalidOperationException($"Away team '{awayTeam.Name}' is not active");
                }
            }

            // Validate match date if changing
            if (command.MatchDate.HasValue)
            {
                var gameweekId = command.GameweekId ?? existingMatch.GameweekId;
                var gameweek = await _gameweekRepository.GetByIdAsync(gameweekId);
                if (gameweek != null)
                {
                    if (command.MatchDate.Value.Date < gameweek.StartDate.Date || 
                        command.MatchDate.Value.Date > gameweek.EndDate.Date)
                    {
                        throw new InvalidOperationException(
                            $"Match date must be between {gameweek.StartDate:yyyy-MM-dd} and {gameweek.EndDate:yyyy-MM-dd}");
                    }
                }
            }

            // Validate scores can only be updated for appropriate match statuses
            if ((command.HomeScore.HasValue || command.AwayScore.HasValue))
            {
                var finalStatus = command.Status ?? existingMatch.Status;
                if (!existingMatch.CanUpdateScore() && finalStatus != Domain.Enums.MatchStatus.Live && finalStatus != Domain.Enums.MatchStatus.Finished)
                {
                    throw new InvalidOperationException("Scores can only be updated for live or finished matches");
                }
            }
        }

        private static void ValidatePaginationFilter(MatchFilterDto filter)
        {
            if (filter.Page < 1)
                throw new ArgumentException("Page must be greater than or equal to 1", nameof(filter.Page));

            if (filter.Limit < 1 || filter.Limit > 100)
                throw new ArgumentException("Limit must be between 1 and 100", nameof(filter.Limit));

            if (!string.IsNullOrEmpty(filter.Sort) && 
                filter.Sort != "match_date" && filter.Sort != "gameweek_number")
            {
                throw new ArgumentException("Sort must be 'match_date' or 'gameweek_number'", nameof(filter.Sort));
            }

            if (!string.IsNullOrEmpty(filter.Order) && 
                filter.Order != "asc" && filter.Order != "desc")
            {
                throw new ArgumentException("Order must be 'asc' or 'desc'", nameof(filter.Order));
            }

            if (filter.DateFrom.HasValue && filter.DateTo.HasValue)
            {
                if (filter.DateFrom.Value.Date > filter.DateTo.Value.Date)
                {
                    throw new ArgumentException("DateFrom cannot be greater than DateTo");
                }
            }
        }

        private static MatchDto MapToDto(Match match)
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

        private static TeamDto MapTeamToDto(Team team)
        {
            return new TeamDto
            {
                Id = team.Id,
                Name = team.Name,
                ShortCode = team.ShortCode,
                CrestUrl = team.CrestUrl
            };
        }

        private static GameweekDto MapGameweekToDto(Gameweek gameweek)
        {
            return new GameweekDto
            {
                Id = gameweek.Id,
                Number = gameweek.Number,
                StartDate = gameweek.StartDate,
                EndDate = gameweek.EndDate,
                Status = gameweek.GetStatus()
            };
        }
    }
}
