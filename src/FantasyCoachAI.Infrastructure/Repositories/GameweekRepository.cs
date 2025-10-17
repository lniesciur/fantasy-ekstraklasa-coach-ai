using FantasyCoachAI.Domain.Entities;
using FantasyCoachAI.Domain.Enums;
using FantasyCoachAI.Domain.Interfaces;
using FantasyCoachAI.Infrastructure.Mappers;
using FantasyCoachAI.Infrastructure.Persistence.SupabaseModels;
using static Supabase.Postgrest.Constants;

namespace FantasyCoachAI.Infrastructure.Repositories
{
    public class GameweekRepository : IGameweekRepository
    {
        private readonly Supabase.Client _supabase;

        public GameweekRepository(Supabase.Client supabase)
        {
            _supabase = supabase;
        }

        public async Task<Gameweek?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Id must be greater than zero", nameof(id));

            await _supabase.InitializeAsync();

            var dbModel = await _supabase
                .From<GameweekDbModel>()
                .Select("*, matches:matches(*, home_team:teams!matches_home_team_id_fkey(*), away_team:teams!matches_away_team_id_fkey(*))")
                .Where(g => g.Id == id)
                .Single();

            return dbModel?.ToDomain();
        }

        public async Task<Gameweek?> GetByNumberAsync(int number)
        {
            if (number <= 0)
                throw new ArgumentException("Number must be greater than zero", nameof(number));

            await _supabase.InitializeAsync();

            var dbModel = await _supabase
                .From<GameweekDbModel>()
                .Where(g => g.Number == number)
                .Single();

            return dbModel?.ToDomain();
        }

        public async Task<Gameweek> CreateAsync(Gameweek gameweek)
        {
            if (gameweek == null)
                throw new ArgumentNullException(nameof(gameweek));

            if (!gameweek.IsValidDateRange())
                throw new ArgumentException("Start date must be before end date", nameof(gameweek));

            await _supabase.InitializeAsync();

            var insertDbModel = gameweek.ToInsertDbModel();

            // Insertuj używając modelu bez PrimaryKey
            await _supabase
                .From<GameweekInsertDbModel>()
                .Insert(insertDbModel);

            // Pobierz dodany rekord używając pełnego modelu (PostgREST bug workaround)
            var addedRecord = await _supabase
                .From<GameweekDbModel>()
                .Where(g => g.Number == gameweek.Number)
                .Single();

            return addedRecord?.ToDomain() ?? throw new InvalidOperationException("Failed to retrieve inserted gameweek record");
        }

        public async Task DeleteAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Id must be greater than zero", nameof(id));

            await _supabase.InitializeAsync();

            await _supabase
                .From<GameweekDbModel>()
                .Where(g => g.Id == id)
                .Delete();
        }

        public async Task<List<Gameweek>> GetFilteredAsync(
            GameweekStatus? status = null,
            string? sortBy = "number",
            bool ascending = true)
        {
            await _supabase.InitializeAsync();

            var query = _supabase
                .From<GameweekDbModel>()
                .Select("*, matches:matches(*, home_team:teams!matches_home_team_id_fkey(*), away_team:teams!matches_away_team_id_fkey(*))");

            // Sortowanie
            var orderBy = sortBy?.ToLower() switch
            {
                "start_date" => ascending
                    ? query.Order(g => g.StartDate, Ordering.Ascending)
                    : query.Order(g => g.StartDate, Ordering.Descending),
                _ => ascending
                    ? query.Order(g => g.Number, Ordering.Ascending)
                    : query.Order(g => g.Number, Ordering.Descending)
            };

            var response = await orderBy.Get();
            var gameweeks = response.Models.Select(dbModel => dbModel.ToDomain()).ToList();

            // Filtrowanie według statusu (po pobraniu z bazy, bo status jest obliczany)
            if (status.HasValue)
            {
                gameweeks = gameweeks.Where(g => g.GetStatus() == status.Value).ToList();
            }

            return gameweeks;
        }
    }
}
