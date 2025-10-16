using FantasyCoachAI.Domain.Entities;
using FantasyCoachAI.Domain.Enums;

namespace FantasyCoachAI.Domain.Interfaces
{
    public interface IMatchRepository
    {
        Task<List<Match>> GetAllAsync();
        Task<Match?> GetByIdAsync(int id);
        Task<Match> CreateAsync(Match match);
        Task UpdateAsync(Match match);
        
        Task<(List<Match> Matches, int TotalCount)> GetMatchesAsync(
            int? gameweekId = null,
            int? teamId = null, 
            MatchStatus? status = null,
            DateTime? dateFrom = null,
            DateTime? dateTo = null,
            string sortBy = "match_date",
            bool ascending = true,
            int page = 1,
            int limit = 50);

        Task<List<Match>> GetByGameweekIdAsync(int gameweekId);
        Task<List<Match>> GetByTeamIdAsync(int teamId);
        Task<bool> ExistsMatchBetweenTeamsInGameweekAsync(int homeTeamId, int awayTeamId, int gameweekId);
        Task<bool> ExistsAsync(int id);
    }
}
