using FantasyCoachAI.Domain.Interfaces;
using FantasyCoachAI.Infrastructure.Persistence.SupabaseModels;
using Supabase.Postgrest;
using static Supabase.Postgrest.Constants;

namespace FantasyCoachAI.Infrastructure.Repositories;

public class PlayerStatsRepository : IPlayerStatsRepository
{
    private readonly Supabase.Client _supabase;

    public PlayerStatsRepository(Supabase.Client supabase)
    {
        _supabase = supabase;
    }

    public async Task<(List<dynamic> data, int total)> GetFilteredAsync(
        int? matchId,
        int? playerId = null,
        int? teamId = null,
        string? position = null,
        string sort = "fantasy_points",
        string order = "desc",
        int limit = 50,
        int page = 1)
    {
        await _supabase.InitializeAsync();

        // Build base select with player and match relationships
        const string baseSelect = "*, player:players!player_stats_player_id_fkey(id,name,position,team_id), match:matches!player_stats_match_id_fkey(id,gameweek_id,gameweek:gameweeks(id,number))";
        
        // Build query with JOINs to get player and gameweek information
        var query = _supabase
            .From<PlayerStatsDbModel>()
            .Select(baseSelect);

        // Apply match filter if provided
        if (matchId.HasValue)
            query = query.Where(ps => ps.MatchId == matchId.Value);
        else
            query = query.Where(ps => ps.MatchId == null);

        // Apply optional filters
        if (playerId.HasValue)
            query = query.Where(ps => ps.PlayerId == playerId.Value);

        if (teamId.HasValue)
        {
            // Filter by team without overriding the select
            query = query.Filter("player.team_id", Operator.Equals, teamId.Value);
        }

        if (!string.IsNullOrEmpty(position))
        {
            // Filter by position without overriding the select
            query = query.Filter("player.position", Operator.Equals, position);
        }

        // Apply sorting
        var ordering = order?.ToLower() == "asc" ? Ordering.Ascending : Ordering.Descending;
        query = sort?.ToLower() switch
        {
            "form" => query.Order(ps => ps.Price, ordering),
            "price" => query.Order(ps => ps.Price, ordering),
            _ => query.Order(ps => ps.FantasyPoints, ordering)
        };

        // Apply pagination
        var offset = (page - 1) * limit;
        query = query.Range(offset, offset + limit - 1);

        // Execute query
        var response = await query.Get();

        // Count total records for pagination
        var countQuery = _supabase
            .From<PlayerStatsDbModel>()
            .Select(baseSelect);

        // Apply same filters for count
        if (matchId.HasValue)
            countQuery = countQuery.Where(ps => ps.MatchId == matchId.Value);
        else
            countQuery = countQuery.Where(ps => ps.MatchId == null);

        if (playerId.HasValue)
            countQuery = countQuery.Where(ps => ps.PlayerId == playerId.Value);

        if (teamId.HasValue)
            countQuery = countQuery.Filter("player.team_id", Operator.Equals, teamId.Value);

        if (!string.IsNullOrEmpty(position))
            countQuery = countQuery.Filter("player.position", Operator.Equals, position);

        var countResponse = await countQuery.Get();
        var total = countResponse.Models.Count;

        // Return as dynamic for mapping in service layer
        var dtos = response.Models
            .Cast<dynamic>()
            .ToList();

        return (dtos, total);
    }

    public async Task<(List<dynamic> data, int total)> GetStatsByGameweekAsync(
        int gameweekId,
        int? playerId = null,
        int? teamId = null,
        string? position = null,
        string sort = "fantasy_points",
        string order = "desc",
        int limit = 50,
        int page = 1)
    {
        await _supabase.InitializeAsync();

        // Build base select with player and match relationships
        const string baseSelect = "*, player:players!player_stats_player_id_fkey(id,name,position,team_id), match:matches!player_stats_match_id_fkey(id,gameweek_id,gameweek:gameweeks(id,number))";
        
        // Build query with JOINs to get player and gameweek information
        var query = _supabase
            .From<PlayerStatsDbModel>()
            .Select(baseSelect)
            .Where(ps => ps.MatchId != null);

        // Apply gameweek filter through matches table
        query = query.Filter("match.gameweek_id", Operator.Equals, gameweekId);

        // Apply optional filters
        if (playerId.HasValue)
            query = query.Where(ps => ps.PlayerId == playerId.Value);

        if (teamId.HasValue)
            query = query.Filter("player.team_id", Operator.Equals, teamId.Value);

        if (!string.IsNullOrEmpty(position))
            query = query.Filter("player.position", Operator.Equals, position);

        // Apply sorting
        var ordering = order?.ToLower() == "asc" ? Ordering.Ascending : Ordering.Descending;
        query = sort?.ToLower() switch
        {
            "form" => query.Order(ps => ps.Price, ordering),
            "price" => query.Order(ps => ps.Price, ordering),
            _ => query.Order(ps => ps.FantasyPoints, ordering)
        };

        // Apply pagination
        var offset = (page - 1) * limit;
        query = query.Range(offset, offset + limit - 1);

        // Execute query
        var response = await query.Get();

        // Count total records for pagination
        var countQuery = _supabase
            .From<PlayerStatsDbModel>()
            .Select(baseSelect)
            .Where(ps => ps.MatchId != null)
            .Filter("match.gameweek_id", Operator.Equals, gameweekId);

        if (playerId.HasValue)
            countQuery = countQuery.Where(ps => ps.PlayerId == playerId.Value);

        if (teamId.HasValue)
            countQuery = countQuery.Filter("player.team_id", Operator.Equals, teamId.Value);

        if (!string.IsNullOrEmpty(position))
            countQuery = countQuery.Filter("player.position", Operator.Equals, position);

        var countResponse = await countQuery.Get();
        var total = countResponse.Models.Count;

        // Return as dynamic for mapping in service layer
        var dtos = response.Models
            .Cast<dynamic>()
            .ToList();

        return (dtos, total);
    }

    public async Task ImportAsync(List<dynamic> stats)
    {
        if (stats == null || !stats.Any())
            throw new ArgumentException("Stats list cannot be null or empty", nameof(stats));

        await _supabase.InitializeAsync();

        try
        {
            // Map dynamic objects to DB models and insert
            var dbModels = stats.Select(s => new PlayerStatsDbModel
            {
                PlayerId = (int)s.PlayerId,
                MatchId = (int?)s.MatchId,
                FantasyPoints = (int)s.FantasyPoints,
                MinutesPlayed = (int)s.MinutesPlayed,
                Goals = (int)s.Goals,
                Assists = (int)s.Assists,
                YellowCards = (int)s.YellowCards,
                RedCards = (int)s.RedCards,
                Saves = (int)s.Saves,
                PenaltiesSaved = (int)s.PenaltiesSaved,
                PenaltiesWon = (int)s.PenaltiesWon,
                PenaltiesScored = (int)s.PenaltiesScored,
                PenaltiesCaused = (int)s.PenaltiesCaused,
                PenaltiesMissed = (int)s.PenaltiesMissed,
                LottoAssists = (int)s.LottoAssists,
                OwnGoals = (int)s.OwnGoals,
                InTeamOfWeek = (bool)s.InTeamOfWeek,
                Price = (decimal)s.Price,
                PredictedStart = (bool)s.PredictedStart,
                HealthStatus = (string)s.HealthStatus,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }).ToList();

            // Batch insert (upsert on conflict for player_id + match_id)
            await _supabase
                .From<PlayerStatsDbModel>()
                .Upsert(dbModels);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to import player statistics: {ex.Message}", ex);
        }
    }

}
