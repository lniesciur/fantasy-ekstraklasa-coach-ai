using FantasyCoachAI.Domain.Entities;
using FantasyCoachAI.Domain.Interfaces;
using FantasyCoachAI.Infrastructure.Mappers;
using FantasyCoachAI.Infrastructure.Persistence.SupabaseModels;
using Supabase.Postgrest;
using static Supabase.Postgrest.Constants;

namespace FantasyCoachAI.Infrastructure.Repositories
{
    public class TeamRepository : ITeamRepository
    {
        private readonly Supabase.Client _supabase;

        public TeamRepository(Supabase.Client supabase)
        {
            _supabase = supabase;
        }

        public async Task<List<Team>> GetAllAsync()
        {
            await _supabase.InitializeAsync();

            var response = await _supabase
                .From<TeamDbModel>()
                .Order(t => t.Name, Ordering.Ascending)
                .Get();

            return response.Models
                .Select(dbModel => dbModel.ToDomain())
                .ToList();
        }

        public async Task<Team?> GetByIdAsync(int id)
        {
            await _supabase.InitializeAsync();

            var dbModel = await _supabase
                .From<TeamDbModel>()
                .Where(t => t.Id == id)
                .Single();

            return dbModel?.ToDomain();
        }

        public async Task<Team> CreateAsync(Team team)
        {
            await _supabase.InitializeAsync();

            if (!team.IsActive)
            {
                team.IsActive = true;
            }

            var insertDbModel = team.ToInsertDbModel();

            // Insertuj używając modelu bez PrimaryKey
            await _supabase
                .From<TeamInsertDbModel>()
                .Insert(insertDbModel);

            // Pobierz dodany rekord używając pełnego modelu (PostgREST bug workaround)
            var addedRecord = await _supabase
                .From<TeamDbModel>()
                .Where(t => t.Name == team.Name && t.ShortCode == team.ShortCode)
                .Single();

            return addedRecord?.ToDomain() ?? throw new InvalidOperationException("Failed to retrieve inserted team record");
        }

        public async Task UpdateAsync(Team team)
        {
            await _supabase.InitializeAsync();

            var dbModel = team.ToDbModel();
            await _supabase.From<TeamDbModel>().Update(dbModel);
        }

        public async Task<List<Team>> GetFilteredAsync(bool? isActive = null, string? shortCode = null)
        {
            await _supabase.InitializeAsync();

            var query = _supabase
                .From<TeamDbModel>()
                .Select("*");

            if (isActive.HasValue)
            {
                query = query.Filter("is_active", Supabase.Postgrest.Constants.Operator.Equals, isActive.Value.ToString().ToLower());
            }

            if (!string.IsNullOrWhiteSpace(shortCode))
            {
                query = query.Filter("short_code", Supabase.Postgrest.Constants.Operator.ILike, shortCode);
            }

            query = query.Order("name", Supabase.Postgrest.Constants.Ordering.Ascending);

            var response = await query.Get();

            return response.Models
                .Select(dbModel => dbModel.ToDomain())
                .ToList();
        }
    }
}
