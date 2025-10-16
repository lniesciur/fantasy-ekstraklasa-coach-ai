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

        public async Task<List<Gameweek>> GetAllAsync()
        {
            await _supabase.InitializeAsync();

            var response = await _supabase
                .From<GameweekDbModel>()
                .Order(g => g.Number, Ordering.Ascending)
                .Get();

            return response.Models
                .Select(dbModel => dbModel.ToDomain())
                .ToList();
        }

        public async Task<Gameweek?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Id must be greater than zero", nameof(id));

            await _supabase.InitializeAsync();

            var dbModel = await _supabase
                .From<GameweekDbModel>()
                .Where(g => g.Id == id)
                .Single();

            return dbModel?.ToDomain();
        }

        public async Task<Gameweek?> GetCurrentAsync()
        {
            await _supabase.InitializeAsync();

            var now = DateTime.UtcNow.Date;

            var response = await _supabase
                .From<GameweekDbModel>()
                .Where(g => g.StartDate <= now && g.EndDate >= now)
                .Single();

            return response?.ToDomain();
        }

        public async Task<Gameweek> CreateAsync(Gameweek gameweek)
        {
            if (gameweek == null)
                throw new ArgumentNullException(nameof(gameweek));

            if (!gameweek.IsValidDateRange())
                throw new ArgumentException("Start date must be before end date", nameof(gameweek));

            await _supabase.InitializeAsync();

            var dbModel = gameweek.ToDbModel();

            var response = await _supabase
                .From<GameweekDbModel>()
                .Insert(dbModel);

            return response.Models.First().ToDomain();
        }

        public async Task UpdateAsync(Gameweek gameweek)
        {
            if (gameweek == null)
                throw new ArgumentNullException(nameof(gameweek));

            if (gameweek.Id <= 0)
                throw new ArgumentException("Id must be greater than zero", nameof(gameweek));

            if (!gameweek.IsValidDateRange())
                throw new ArgumentException("Start date must be before end date", nameof(gameweek));

            await _supabase.InitializeAsync();

            var dbModel = gameweek.ToDbModel();
            await _supabase.From<GameweekDbModel>().Update(dbModel);
        }

        public async Task<List<Gameweek>> GetFilteredAsync(
            GameweekStatus? status = null, 
            string? sortBy = "number", 
            bool ascending = true)
        {
            await _supabase.InitializeAsync();

            var query = _supabase.From<GameweekDbModel>();

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
