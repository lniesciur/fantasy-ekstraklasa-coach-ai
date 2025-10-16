using FantasyCoachAI.Domain.Entities;
using FantasyCoachAI.Domain.Enums;
using FantasyCoachAI.Domain.Interfaces;
using FantasyCoachAI.Infrastructure.Mappers;
using FantasyCoachAI.Infrastructure.Persistence.SupabaseModels;
using static Supabase.Postgrest.Constants;

namespace FantasyCoachAI.Infrastructure.Repositories
{
    public class MatchRepository : IMatchRepository
    {
        private readonly Supabase.Client _supabase;

        public MatchRepository(Supabase.Client supabase)
        {
            _supabase = supabase;
        }

        public async Task<List<Match>> GetAllAsync()
        {
            await _supabase.InitializeAsync();

            var response = await _supabase
                .From<MatchDbModel>()
                .Order(m => m.MatchDate, Ordering.Ascending)
                .Get();

            return response.Models
                .Select(dbModel => dbModel.ToDomain())
                .ToList();
        }

        public async Task<Match?> GetByIdAsync(int id)
        {
            await _supabase.InitializeAsync();

            var query = _supabase
                .From<MatchDbModel>()
                .Select("*, gameweek:gameweeks(*), home_team:teams!matches_home_team_id_fkey(*), away_team:teams!matches_away_team_id_fkey(*)")
                .Where(m => m.Id == id);

            var dbModel = await query.Single();

            return dbModel?.ToDomain();
        }

        public async Task<Match> CreateAsync(Match match)
        {
            await _supabase.InitializeAsync();

            match.CreatedAt = DateTime.UtcNow;
            match.UpdatedAt = DateTime.UtcNow;

            var dbModel = match.ToDbModel();

            var response = await _supabase
                .From<MatchDbModel>()
                .Insert(dbModel);

            return response.Models.First().ToDomain();
        }

        public async Task UpdateAsync(Match match)
        {
            await _supabase.InitializeAsync();

            match.UpdatedAt = DateTime.UtcNow;

            var dbModel = match.ToDbModel();
            await _supabase.From<MatchDbModel>().Update(dbModel);
        }

        public async Task<(List<Match> Matches, int TotalCount)> GetMatchesAsync(
            int? gameweekId = null,
            int? teamId = null,
            MatchStatus? status = null,
            DateTime? dateFrom = null,
            DateTime? dateTo = null,
            string sortBy = "match_date",
            bool ascending = true,
            int page = 1,
            int limit = 50)
        {
            await _supabase.InitializeAsync();

            // Build query for matches with joins
            var query = _supabase
                .From<MatchDbModel>()
                .Select("*, gameweek:gameweeks(*), home_team:teams!matches_home_team_id_fkey(*), away_team:teams!matches_away_team_id_fkey(*)");

            // Apply filters
            if (gameweekId.HasValue)
            {
                query = query.Filter("gameweek_id", Operator.Equals, gameweekId.Value.ToString());
            }

            if (teamId.HasValue)
            {
                query = query.Filter("or", Operator.Equals, $"(home_team_id.eq.{teamId.Value},away_team_id.eq.{teamId.Value})");
            }

            if (status.HasValue)
            {
                var dbStatus = MapStatusToDb(status.Value);
                query = query.Filter("status", Operator.Equals, dbStatus);
            }

            if (dateFrom.HasValue)
            {
                query = query.Filter("match_date", Operator.GreaterThanOrEqual, dateFrom.Value.ToString("yyyy-MM-dd"));
            }

            if (dateTo.HasValue)
            {
                query = query.Filter("match_date", Operator.LessThanOrEqual, dateTo.Value.ToString("yyyy-MM-dd"));
            }

            // Get total count by fetching all matching records (without pagination)
            var countQuery = _supabase
                .From<MatchDbModel>()
                .Select("*");

            // Apply same filters for count
            if (gameweekId.HasValue)
            {
                countQuery = countQuery.Filter("gameweek_id", Operator.Equals, gameweekId.Value.ToString());
            }

            if (teamId.HasValue)
            {
                countQuery = countQuery.Filter("or", Operator.Equals, $"(home_team_id.eq.{teamId.Value},away_team_id.eq.{teamId.Value})");
            }

            if (status.HasValue)
            {
                var dbStatus = MapStatusToDb(status.Value);
                countQuery = countQuery.Filter("status", Operator.Equals, dbStatus);
            }

            if (dateFrom.HasValue)
            {
                countQuery = countQuery.Filter("match_date", Operator.GreaterThanOrEqual, dateFrom.Value.ToString("yyyy-MM-dd"));
            }

            if (dateTo.HasValue)
            {
                countQuery = countQuery.Filter("match_date", Operator.LessThanOrEqual, dateTo.Value.ToString("yyyy-MM-dd"));
            }

            var countResponse = await countQuery.Get();
            var totalCount = countResponse.Models.Count;

            // Apply sorting
            if (sortBy == "gameweek_number")
            {
                query = ascending
                    ? query.Order("gameweek.number", Ordering.Ascending)
                    : query.Order("gameweek.number", Ordering.Descending);
            }
            else
            {
                query = ascending
                    ? query.Order("match_date", Ordering.Ascending)
                    : query.Order("match_date", Ordering.Descending);
            }

            // Apply pagination
            var offset = (page - 1) * limit;
            query = query.Range(offset, offset + limit - 1);

            var response = await query.Get();

            var matches = response.Models
                .Select(dbModel => dbModel.ToDomain())
                .ToList();

            return (matches, totalCount);
        }

        public async Task<List<Match>> GetByGameweekIdAsync(int gameweekId)
        {
            await _supabase.InitializeAsync();

            var response = await _supabase
                .From<MatchDbModel>()
                .Select("*, gameweek:gameweeks(*), home_team:teams!matches_home_team_id_fkey(*), away_team:teams!matches_away_team_id_fkey(*)")
                .Filter("gameweek_id", Operator.Equals, gameweekId.ToString())
                .Order("match_date", Ordering.Ascending)
                .Get();

            return response.Models
                .Select(dbModel => dbModel.ToDomain())
                .ToList();
        }

        public async Task<List<Match>> GetByTeamIdAsync(int teamId)
        {
            await _supabase.InitializeAsync();

            var response = await _supabase
                .From<MatchDbModel>()
                .Select("*, gameweek:gameweeks(*), home_team:teams!matches_home_team_id_fkey(*), away_team:teams!matches_away_team_id_fkey(*)")
                .Filter("or", Operator.Equals, $"(home_team_id.eq.{teamId},away_team_id.eq.{teamId})")
                .Order("match_date", Ordering.Ascending)
                .Get();

            return response.Models
                .Select(dbModel => dbModel.ToDomain())
                .ToList();
        }

        public async Task<bool> ExistsMatchBetweenTeamsInGameweekAsync(int homeTeamId, int awayTeamId, int gameweekId)
        {
            await _supabase.InitializeAsync();

            var response = await _supabase
                .From<MatchDbModel>()
                .Select("id")
                .Filter("gameweek_id", Operator.Equals, gameweekId.ToString())
                .Filter("home_team_id", Operator.Equals, homeTeamId.ToString())
                .Filter("away_team_id", Operator.Equals, awayTeamId.ToString())
                .Get();

            return response.Models.Any();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            await _supabase.InitializeAsync();

            var response = await _supabase
                .From<MatchDbModel>()
                .Select("id")
                .Filter("id", Operator.Equals, id.ToString())
                .Get();

            return response.Models.Any();
        }

        private static string MapStatusToDb(MatchStatus domainStatus)
        {
            return domainStatus switch
            {
                MatchStatus.Scheduled => "scheduled",
                MatchStatus.Live => "live",
                MatchStatus.Finished => "played",
                MatchStatus.Postponed => "postponed",
                MatchStatus.Cancelled => "cancelled",
                _ => "scheduled"
            };
        }
    }
}
