using FantasyCoachAI.Domain.Entities;
using FantasyCoachAI.Domain.Enums;
using FantasyCoachAI.Infrastructure.Persistence.SupabaseModels;

namespace FantasyCoachAI.Infrastructure.Mappers
{
    public static class MatchMapper
    {
        /// <summary>
        /// Mapuje z modelu bazodanowego do encji domenowej
        /// </summary>
        public static Match ToDomain(this MatchDbModel dbModel)
        {
            return new Match
            {
                Id = dbModel.Id,
                GameweekId = dbModel.GameweekId,
                HomeTeamId = dbModel.HomeTeamId,
                AwayTeamId = dbModel.AwayTeamId,
                MatchDate = dbModel.MatchDate,
                Status = MapStatusToDomain(dbModel.Status),
                HomeScore = dbModel.HomeScore,
                AwayScore = dbModel.AwayScore,
                RescheduleReason = dbModel.RescheduleReason,
                CreatedAt = dbModel.CreatedAt,
                UpdatedAt = dbModel.UpdatedAt,
                Gameweek = dbModel.Gameweek?.ToDomain(),
                HomeTeam = dbModel.HomeTeam?.ToDomain(),
                AwayTeam = dbModel.AwayTeam?.ToDomain()
            };
        }

        /// <summary>
        /// Mapuje z encji domenowej do modelu bazodanowego
        /// </summary>
        public static MatchDbModel ToDbModel(this Match domain)
        {
            return new MatchDbModel
            {
                Id = domain.Id,
                GameweekId = domain.GameweekId,
                HomeTeamId = domain.HomeTeamId,
                AwayTeamId = domain.AwayTeamId,
                MatchDate = domain.MatchDate,
                Status = MapStatusToDb(domain.Status),
                HomeScore = domain.HomeScore,
                AwayScore = domain.AwayScore,
                RescheduleReason = domain.RescheduleReason,
                CreatedAt = domain.CreatedAt,
                UpdatedAt = domain.UpdatedAt
            };
        }

        /// <summary>
        /// Mapuje status z bazy danych do enum domenowego
        /// </summary>
        private static MatchStatus MapStatusToDomain(string dbStatus)
        {
            return dbStatus.ToLower() switch
            {
                "scheduled" => MatchStatus.Scheduled,
                "live" => MatchStatus.Live,
                "played" => MatchStatus.Finished,
                "postponed" => MatchStatus.Postponed,
                "cancelled" => MatchStatus.Cancelled,
                _ => MatchStatus.Scheduled
            };
        }

        /// <summary>
        /// Mapuje status z enum domenowego do bazy danych
        /// </summary>
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
