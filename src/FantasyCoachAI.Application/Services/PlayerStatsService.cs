using FantasyCoachAI.Application.DTOs;
using FantasyCoachAI.Application.Interfaces;
using FantasyCoachAI.Domain.Interfaces;
using CsvHelper;
using System.Globalization;

namespace FantasyCoachAI.Application.Services;

public class PlayerStatsService : IPlayerStatsService
{
    private readonly IPlayerStatsRepository _statsRepository;
    private readonly IGameweekRepository _gameweekRepository;

    public PlayerStatsService(
        IPlayerStatsRepository statsRepository,
        IGameweekRepository gameweekRepository)
    {
        _statsRepository = statsRepository;
        _gameweekRepository = gameweekRepository;
    }

    public async Task<(List<PlayerStatsDto> data, int total)> GetStatsAsync(PlayerStatsFilterDto filter)
    {
        // Validate match exists if provided
        if (filter.MatchId.HasValue)
        {
            if (filter.MatchId <= 0)
                throw new ArgumentException("MatchId must be greater than 0");
        }

        // Limit and Page validation
        if (filter.Limit > 100) filter.Limit = 100;
        if (filter.Limit < 1) filter.Limit = 50;
        if (filter.Page < 1) filter.Page = 1;

        // Get stats from repository
        var (stats, total) = await _statsRepository.GetFilteredAsync(
            matchId: filter.MatchId,
            playerId: filter.PlayerId,
            teamId: filter.TeamId,
            position: filter.Position,
            sort: filter.Sort,
            order: filter.Order,
            limit: filter.Limit,
            page: filter.Page);

        // Map dynamic results to DTOs
        var dtos = stats.Select(MapToPlayerStatsDto).ToList();
        return (dtos, total);
    }

    public async Task<PlayerStatsImportResponseDto> ImportFromCsvAsync(PlayerStatsImportRequestDto request)
    {
        var response = new PlayerStatsImportResponseDto { Success = true };
        var records = new List<PlayerStatsImportDto>();
        var errors = new List<string>();

        try
        {
        // No gameweek validation needed - stats can be imported without gameweek assignment

            using (var stream = request.File.OpenReadStream())
            using (var reader = new StreamReader(stream))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Read();
                csv.ReadHeader();

                int rowNumber = 1;
                while (csv.Read())
                {
                    rowNumber++;
                    try
                    {
                        // Safely read fields with defaults
                        if (!csv.TryGetField<int>("player_id", out var playerId))
                            throw new InvalidOperationException("Missing required field: player_id");

                        if (!csv.TryGetField<int>("fantasy_points", out var fantasyPoints))
                            throw new InvalidOperationException("Missing required field: fantasy_points");

                        if (!csv.TryGetField<int>("minutes_played", out var minutesPlayed))
                            throw new InvalidOperationException("Missing required field: minutes_played");

                        if (!csv.TryGetField<decimal>("price", out var price))
                            throw new InvalidOperationException("Missing required field: price");

                        // Basic validation
                        if (fantasyPoints < 0 || fantasyPoints > 100)
                        {
                            errors.Add($"Row {rowNumber}: Invalid fantasy_points value ({fantasyPoints}), must be 0-100");
                            response.SkippedCount++;
                            continue;
                        }

                        if (minutesPlayed < 0 || minutesPlayed > 120)
                        {
                            errors.Add($"Row {rowNumber}: Invalid minutes_played value ({minutesPlayed}), must be 0-120");
                            response.SkippedCount++;
                            continue;
                        }

                        var record = new PlayerStatsImportDto
                        {
                            PlayerId = playerId,
                            MatchId = csv.GetField<int?>("match_id"),
                            FantasyPoints = fantasyPoints,
                            MinutesPlayed = minutesPlayed,
                            Goals = csv.GetField<int>("goals") > 0 ? csv.GetField<int>("goals") : 0,
                            Assists = csv.GetField<int>("assists") > 0 ? csv.GetField<int>("assists") : 0,
                            YellowCards = csv.GetField<int>("yellow_cards") > 0 ? csv.GetField<int>("yellow_cards") : 0,
                            RedCards = csv.GetField<int>("red_cards") > 0 ? csv.GetField<int>("red_cards") : 0,
                            Saves = csv.GetField<int>("saves") > 0 ? csv.GetField<int>("saves") : 0,
                            PenaltiesSaved = csv.GetField<int>("penalties_saved") > 0 ? csv.GetField<int>("penalties_saved") : 0,
                            PenaltiesWon = csv.GetField<int>("penalties_won") > 0 ? csv.GetField<int>("penalties_won") : 0,
                            PenaltiesScored = csv.GetField<int>("penalties_scored") > 0 ? csv.GetField<int>("penalties_scored") : 0,
                            PenaltiesCaused = csv.GetField<int>("penalties_caused") > 0 ? csv.GetField<int>("penalties_caused") : 0,
                            PenaltiesMissed = csv.GetField<int>("penalties_missed") > 0 ? csv.GetField<int>("penalties_missed") : 0,
                            LottoAssists = csv.GetField<int>("lotto_assists") > 0 ? csv.GetField<int>("lotto_assists") : 0,
                            OwnGoals = csv.GetField<int>("own_goals") > 0 ? csv.GetField<int>("own_goals") : 0,
                            InTeamOfWeek = csv.GetField<bool>("in_team_of_week"),
                            Price = price,
                            PredictedStart = csv.GetField<bool>("predicted_start"),
                            HealthStatus = csv.GetField<string>("health_status") ?? "Pewny"
                        };

                        records.Add(record);
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Row {rowNumber}: {ex.Message}");
                        response.SkippedCount++;
                    }
                }
            }

            // Save to database
            if (records.Any())
            {
                await _statsRepository.ImportAsync(records.Cast<dynamic>().ToList());
                response.ImportedCount = records.Count;
            }

            response.Errors = errors;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Errors.Add($"Import failed: {ex.Message}");
        }

        return response;
    }

    /// <summary>
    /// Maps dynamic object (PlayerStatsDbModel) to PlayerStatsDto
    /// </summary>
    private static PlayerStatsDto MapToPlayerStatsDto(dynamic dbModel)
    {
        return new PlayerStatsDto
        {
            Id = dbModel.Id,
            FantasyPoints = dbModel.FantasyPoints,
            MinutesPlayed = dbModel.MinutesPlayed,
            Goals = dbModel.Goals,
            Assists = dbModel.Assists,
            YellowCards = dbModel.YellowCards,
            RedCards = dbModel.RedCards,
            Saves = dbModel.Saves,
            PenaltiesSaved = dbModel.PenaltiesSaved,
            PenaltiesWon = dbModel.PenaltiesWon,
            PenaltiesScored = dbModel.PenaltiesScored,
            PenaltiesCaused = dbModel.PenaltiesCaused,
            PenaltiesMissed = dbModel.PenaltiesMissed,
            LottoAssists = dbModel.LottoAssists,
            OwnGoals = dbModel.OwnGoals,
            InTeamOfWeek = dbModel.InTeamOfWeek,
            Price = dbModel.Price,
            PredictedStart = dbModel.PredictedStart,
            HealthStatus = dbModel.HealthStatus,
            // Map player information from JOIN
            Player = new PlayerStatsPlayerDto 
            { 
                Id = dbModel.PlayerId,
                Name = dbModel.Player?.Name ?? "Unknown",
                Position = dbModel.Player?.Position ?? "Unknown"
            },
            // Map gameweek information from JOIN
            Gameweek = dbModel.Match?.Gameweek != null ? new GameweekStatsDto
            {
                Id = dbModel.Match.Gameweek.Id,
                Number = dbModel.Match.Gameweek.Number
            } : null
        };
    }
}
