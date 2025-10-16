using FantasyCoachAI.Application.Interfaces;
using FantasyCoachAI.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using FantasyCoachAI.Application.Validators;

namespace FantasyCoachAI.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<ITeamService, TeamService>();
            services.AddScoped<IGameweekService, GameweekService>();
            services.AddScoped<IMatchService, MatchService>();

            services.AddValidatorsFromAssemblyContaining<CreateMatchCommandValidator>();

            return services;
        }
    }
}
