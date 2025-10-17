using FantasyCoachAI.Domain.Entities;
using FantasyCoachAI.Infrastructure.Persistence.SupabaseModels;

namespace FantasyCoachAI.Infrastructure.Mappers
{
    public static class TeamMapper
    {
        /// <summary>
        /// Mapuje z modelu bazodanowego do encji domenowej
        /// </summary>
        public static Team ToDomain(this TeamDbModel dbModel)
        {
            return new Team
            {
                Id = dbModel.Id,
                Name = dbModel.Name,
                ShortCode = dbModel.ShortCode,
                CrestUrl = dbModel.CrestUrl,
                IsActive = dbModel.IsActive
            };
        }

        /// <summary>
        /// Mapuje z encji domenowej do modelu bazodanowego (do odczytu i aktualizacji)
        /// </summary>
        public static TeamDbModel ToDbModel(this Team domain)
        {
            var dbModel = new TeamDbModel
            {
                Name = domain.Name,
                ShortCode = domain.ShortCode,
                CrestUrl = domain.CrestUrl,
                IsActive = domain.IsActive
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
        public static TeamInsertDbModel ToInsertDbModel(this Team domain)
        {
            return new TeamInsertDbModel
            {
                Name = domain.Name,
                ShortCode = domain.ShortCode,
                CrestUrl = domain.CrestUrl,
                IsActive = domain.IsActive
            };
        }
    }
}
