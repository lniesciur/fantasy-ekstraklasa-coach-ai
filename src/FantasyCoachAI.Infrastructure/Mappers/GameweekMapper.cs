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
                EndDate = dbModel.EndDate
            };
        }

        /// <summary>
        /// Mapuje z encji domenowej do modelu bazodanowego
        /// </summary>
        public static GameweekDbModel ToDbModel(this Gameweek domain)
        {
            return new GameweekDbModel
            {
                Id = domain.Id,
                Number = domain.Number,
                StartDate = domain.StartDate,
                EndDate = domain.EndDate
            };
        }
    }
}
