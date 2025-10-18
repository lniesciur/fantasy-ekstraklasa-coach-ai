using FantasyCoachAI.Domain.Entities;
using FantasyCoachAI.Domain.Enums;
using FantasyCoachAI.Application.DTOs;

namespace FantasyCoachAI.Application.Tests
{
    /// <summary>
    /// Builder class for creating test data objects
    /// </summary>
    public static class TestDataBuilder
    {
        #region Team Test Data

        /// <summary>
        /// Creates a valid Team entity for testing
        /// </summary>
        public static Team CreateValidTeam(int id = 1, string name = "Test Team", string shortCode = "TST")
        {
            return new Team
            {
                Id = id,
                Name = name,
                ShortCode = shortCode,
                IsActive = true,
                CrestUrl = $"https://example.com/crest_{id}.png"
            };
        }

        /// <summary>
        /// Creates a list of teams for testing
        /// </summary>
        public static List<Team> CreateTeamList(int count = 5)
        {
            return Enumerable.Range(1, count)
                .Select(i => CreateValidTeam(i, $"Team {i}", $"T{i:D2}"))
                .ToList();
        }

        /// <summary>
        /// Creates a large list of teams for performance testing
        /// </summary>
        public static List<Team> CreateLargeTeamList(int count = 10000)
        {
            return Enumerable.Range(1, count)
                .Select(i => new Team
                {
                    Id = i,
                    Name = $"Team {i}",
                    ShortCode = $"T{i:D3}",
                    IsActive = i % 2 == 0,
                    CrestUrl = $"https://example.com/crest_{i}.png"
                })
                .ToList();
        }

        /// <summary>
        /// Creates a CreateTeamCommand for testing
        /// </summary>
        public static CreateTeamCommand CreateCreateTeamCommand(string name = "New Team", string shortCode = "NEW")
        {
            return new CreateTeamCommand
            {
                Name = name,
                ShortCode = shortCode
            };
        }

        /// <summary>
        /// Creates an UpdateTeamCommand for testing
        /// </summary>
        public static UpdateTeamCommand CreateUpdateTeamCommand(int id = 1, bool isActive = true)
        {
            return new UpdateTeamCommand
            {
                Id = id,
                IsActive = isActive
            };
        }

        /// <summary>
        /// Creates a TeamFilterDto for testing
        /// </summary>
        public static TeamFilterDto CreateTeamFilterDto(bool? isActive = true, string? shortCode = null, string sort = "name", string order = "asc")
        {
            return new TeamFilterDto
            {
                IsActive = isActive,
                ShortCode = shortCode,
                Sort = sort,
                Order = order
            };
        }

        #endregion

        #region Gameweek Test Data

        /// <summary>
        /// Creates a valid Gameweek entity for testing
        /// </summary>
        public static Gameweek CreateValidGameweek(int id = 1, int number = 1, DateTime? startDate = null, DateTime? endDate = null)
        {
            var start = startDate ?? DateTime.UtcNow.AddDays(1);
            var end = endDate ?? start.AddDays(6);

            return new Gameweek
            {
                Id = id,
                Number = number,
                StartDate = start,
                EndDate = end
            };
        }

        /// <summary>
        /// Creates a list of gameweeks for testing
        /// </summary>
        public static List<Gameweek> CreateGameweekList(int count = 5)
        {
            return Enumerable.Range(1, count)
                .Select(i => CreateValidGameweek(i, i, DateTime.UtcNow.AddDays(i * 7), DateTime.UtcNow.AddDays((i * 7) + 6)))
                .ToList();
        }

        /// <summary>
        /// Creates a CreateGameweekCommand for testing
        /// </summary>
        public static CreateGameweekCommand CreateCreateGameweekCommand(int number = 1, DateTime? startDate = null, DateTime? endDate = null)
        {
            var start = startDate ?? DateTime.UtcNow.AddDays(1);
            var end = endDate ?? start.AddDays(6);

            return new CreateGameweekCommand
            {
                Number = number,
                StartDate = start,
                EndDate = end
            };
        }

        /// <summary>
        /// Creates an UpdateGameweekCommand for testing
        /// </summary>
        public static UpdateGameweekCommand CreateUpdateGameweekCommand(int id = 1, int number = 1, DateTime? startDate = null, DateTime? endDate = null)
        {
            var start = startDate ?? DateTime.UtcNow.AddDays(1);
            var end = endDate ?? start.AddDays(6);

            return new UpdateGameweekCommand
            {
                Id = id,
                Number = number,
                StartDate = start,
                EndDate = end
            };
        }

        /// <summary>
        /// Creates a GameweekFilterDto for testing
        /// </summary>
        public static GameweekFilterDto CreateGameweekFilterDto(GameweekStatus? status = null, string sort = "start_date", string order = "asc")
        {
            return new GameweekFilterDto
            {
                Status = status,
                Sort = sort,
                Order = order
            };
        }

        #endregion

        #region Match Test Data

        /// <summary>
        /// Creates a valid Match entity for testing
        /// </summary>
        public static FantasyCoachAI.Domain.Entities.Match CreateValidMatch(int id = 1, Team? homeTeam = null, Team? awayTeam = null, DateTime? matchDate = null, MatchStatus status = MatchStatus.Scheduled)
        {
            var home = homeTeam ?? CreateValidTeam(1, "Home Team", "HOM");
            var away = awayTeam ?? CreateValidTeam(2, "Away Team", "AWY");
            var date = matchDate ?? DateTime.UtcNow.AddDays(1);

            return new FantasyCoachAI.Domain.Entities.Match
            {
                Id = id,
                HomeTeam = home,
                AwayTeam = away,
                MatchDate = date,
                Status = status
            };
        }

        /// <summary>
        /// Creates a list of matches for testing
        /// </summary>
        public static List<FantasyCoachAI.Domain.Entities.Match> CreateMatchList(int count = 5)
        {
            var teams = CreateTeamList(count * 2);
            return Enumerable.Range(1, count)
                .Select(i => CreateValidMatch(
                    i,
                    teams[i * 2 - 2],
                    teams[i * 2 - 1],
                    DateTime.UtcNow.AddDays(i),
                    MatchStatus.Scheduled))
                .ToList();
        }

        /// <summary>
        /// Creates a CreateMatchCommand for testing
        /// </summary>
        public static CreateMatchCommand CreateCreateMatchCommand(int homeTeamId = 1, int awayTeamId = 2, DateTime? matchDate = null)
        {
            var date = matchDate ?? DateTime.UtcNow.AddDays(1);

            return new CreateMatchCommand
            {
                HomeTeamId = homeTeamId,
                AwayTeamId = awayTeamId,
                MatchDate = date
            };
        }

        /// <summary>
        /// Creates an UpdateMatchCommand for testing
        /// </summary>
        public static UpdateMatchCommand CreateUpdateMatchCommand(int id = 1, int homeTeamId = 1, int awayTeamId = 2, DateTime? matchDate = null, MatchStatus status = MatchStatus.Scheduled)
        {
            var date = matchDate ?? DateTime.UtcNow.AddDays(1);

            return new UpdateMatchCommand
            {
                Id = id,
                HomeTeamId = homeTeamId,
                AwayTeamId = awayTeamId,
                MatchDate = date,
                Status = status
            };
        }

        /// <summary>
        /// Creates a MatchFilterDto for testing
        /// </summary>
        public static MatchFilterDto CreateMatchFilterDto(MatchStatus? status = null, int? gameweekId = null, string sort = "match_date", string order = "asc")
        {
            return new FantasyCoachAI.Application.DTOs.MatchFilterDto
            {
                Status = status,
                GameweekId = gameweekId,
                Sort = sort,
                Order = order
            };
        }

        #endregion

        #region PlayerStats Test Data

        /// <summary>
        /// Creates a valid PlayerStats entity for testing
        /// </summary>
        public static PlayerStats CreateValidPlayerStats(int id = 1, int playerId = 1, int? matchId = null)
        {
            return new PlayerStats
            {
                Id = id,
                PlayerId = playerId,
                MatchId = matchId,
                Goals = 1,
                Assists = 1,
                YellowCards = 0,
                RedCards = 0,
                MinutesPlayed = 90,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }

        /// <summary>
        /// Creates a list of player stats for testing
        /// </summary>
        public static List<PlayerStats> CreatePlayerStatsList(int count = 5)
        {
            return Enumerable.Range(1, count)
                .Select(i => CreateValidPlayerStats(i, i, i))
                .ToList();
        }

        /// <summary>
        /// Creates a PlayerStatsFilterDto for testing
        /// </summary>
        public static PlayerStatsFilterDto CreatePlayerStatsFilterDto(int? playerId = null, int? matchId = null, string sort = "fantasy_points", string order = "desc")
        {
            return new PlayerStatsFilterDto
            {
                PlayerId = playerId,
                MatchId = matchId,
                Sort = sort,
                Order = order
            };
        }

        #endregion

        #region Invalid Data Builders

        /// <summary>
        /// Creates an invalid CreateTeamCommand with empty name
        /// </summary>
        public static CreateTeamCommand CreateInvalidCreateTeamCommand()
        {
            return new CreateTeamCommand
            {
                Name = "", // Invalid empty name
                ShortCode = "INV"
            };
        }

        /// <summary>
        /// Creates an invalid CreateGameweekCommand with start date after end date
        /// </summary>
        public static CreateGameweekCommand CreateInvalidCreateGameweekCommand()
        {
            return new CreateGameweekCommand
            {
                Number = 1,
                StartDate = DateTime.UtcNow.AddDays(7), // Start after end
                EndDate = DateTime.UtcNow.AddDays(1)   // End before start
            };
        }

        /// <summary>
        /// Creates an invalid CreateMatchCommand with same home and away team
        /// </summary>
        public static CreateMatchCommand CreateInvalidCreateMatchCommand()
        {
            return new CreateMatchCommand
            {
                HomeTeamId = 1,
                AwayTeamId = 1, // Same team
                MatchDate = DateTime.UtcNow.AddDays(1)
            };
        }

        #endregion

        #region Edge Case Data

        /// <summary>
        /// Creates a team with maximum allowed values
        /// </summary>
        public static Team CreateMaxValuesTeam(int id = 1)
        {
            return new Team
            {
                Id = id,
                Name = new string('A', 100), // Maximum name length
                ShortCode = "MAX",
                IsActive = true,
                CrestUrl = new string('A', 500) // Long URL
            };
        }

        /// <summary>
        /// Creates a team with minimum values
        /// </summary>
        public static Team CreateMinValuesTeam(int id = 1)
        {
            return new Team
            {
                Id = id,
                Name = "A", // Minimum name length
                ShortCode = "A",
                IsActive = false,
                CrestUrl = null
            };
        }

        /// <summary>
        /// Creates a gameweek with dates in the past
        /// </summary>
        public static Gameweek CreatePastGameweek(int id = 1)
        {
            return new Gameweek
            {
                Id = id,
                Number = 1,
                StartDate = DateTime.UtcNow.AddDays(-7),
                EndDate = DateTime.UtcNow.AddDays(-1)
            };
        }

        /// <summary>
        /// Creates a gameweek with dates in the future
        /// </summary>
        public static Gameweek CreateFutureGameweek(int id = 1)
        {
            return new Gameweek
            {
                Id = id,
                Number = 1,
                StartDate = DateTime.UtcNow.AddDays(30),
                EndDate = DateTime.UtcNow.AddDays(36)
            };
        }

        #endregion
    }
}
