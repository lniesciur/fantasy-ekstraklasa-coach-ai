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
        /// Mapuje z encji domenowej do modelu bazodanowego
        /// </summary>
        public static TeamDbModel ToDbModel(this Team domain)
        {
            return new TeamDbModel
            {
                Id = domain.Id,
                Name = domain.Name,
                ShortCode = domain.ShortCode,
                CrestUrl = domain.CrestUrl,
                IsActive = domain.IsActive
            };
        }
    }
}
