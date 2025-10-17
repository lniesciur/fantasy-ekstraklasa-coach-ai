using FantasyCoachAI.Domain.Entities;
using FantasyCoachAI.Infrastructure.Persistence.SupabaseModels;

namespace FantasyCoachAI.Infrastructure.Mappers
{
    public static class GameweekMapper
    {
        /// <summary>
        /// Mapuje z modelu bazodanowego do encji domenowej
        /// </summary>
        public static Gameweek ToDomain(this GameweekDbModel dbModel)
        {
            return new Gameweek
            {
                Id = dbModel.Id,
                Number = dbModel.Number,
                StartDate = dbModel.StartDate,
                EndDate = dbModel.EndDate,
                Matches = dbModel.Matches?.Select(m => m.ToDomain()).ToList()
            };
        }

        /// <summary>
        /// Mapuje z encji domenowej do modelu bazodanowego (do odczytu i aktualizacji)
        /// </summary>
        public static GameweekDbModel ToDbModel(this Gameweek domain)
        {
            var dbModel = new GameweekDbModel
            {
                Number = domain.Number,
                StartDate = domain.StartDate,
                EndDate = domain.EndDate
            };

            // Tylko ustaw ID jeśli jest większe od 0 (dla aktualizacji)
            // Dla nowych rekordów (ID = 0) pozwól bazie danych wygenerować ID
            if (domain.Id > 0)
            {
                dbModel.Id = domain.Id;
            }

            return dbModel;
        }

        /// <summary>
        /// Mapuje z encji domenowej do modelu bazodanowego do insertu (bez ID)
        /// </summary>
        public static GameweekInsertDbModel ToInsertDbModel(this Gameweek domain)
        {
            return new GameweekInsertDbModel
            {
                Number = domain.Number,
                StartDate = domain.StartDate,
                EndDate = domain.EndDate
            };
        }
    }
}
