using FantasyCoachAI.Domain.Interfaces;
using FantasyCoachAI.Infrastructure.Configuration;
using FantasyCoachAI.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FantasyCoachAI.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var supabaseSettings = new SupabaseSettings();
            configuration.GetSection("Supabase").Bind(supabaseSettings);

            if (string.IsNullOrEmpty(supabaseSettings.Url) ||
                string.IsNullOrEmpty(supabaseSettings.Key))
            {
                throw new InvalidOperationException("Supabase configuration is missing or incomplete");
            }

            services.AddSingleton(supabaseSettings);

            services.AddScoped(provider =>
            {
                var settings = provider.GetRequiredService<SupabaseSettings>();

                return new Supabase.Client(
                    settings.Url,
                    settings.Key,
                    new Supabase.SupabaseOptions
                    {
                        AutoRefreshToken = true,
                        AutoConnectRealtime = false // Włącz jeśli potrzebujesz realtime
                    }
                );
            });

            services.AddScoped<ITeamRepository, TeamRepository>();
            services.AddScoped<IGameweekRepository, GameweekRepository>();
            services.AddScoped<IMatchRepository, MatchRepository>();
            services.AddScoped<IPlayerStatsRepository, PlayerStatsRepository>();

            return services;
        }
    }
}
