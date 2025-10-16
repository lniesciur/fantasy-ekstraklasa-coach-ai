using FantasyCoachAI.Application.DTOs;
using FluentValidation;

namespace FantasyCoachAI.Application.Validators
{
    public class CreateMatchCommandValidator : AbstractValidator<CreateMatchCommand>
    {
        public CreateMatchCommandValidator()
        {
            RuleFor(x => x.GameweekId)
                .GreaterThan(0)
                .WithMessage("GameweekId must be greater than zero");

            RuleFor(x => x.HomeTeamId)
                .GreaterThan(0)
                .WithMessage("HomeTeamId must be greater than zero");

            RuleFor(x => x.AwayTeamId)
                .GreaterThan(0)
                .WithMessage("AwayTeamId must be greater than zero");

            RuleFor(x => x.MatchDate)
                .GreaterThan(DateTime.UtcNow.AddDays(-1))
                .WithMessage("Match date cannot be more than 1 day in the past")
                .LessThan(DateTime.UtcNow.AddYears(2))
                .WithMessage("Match date cannot be more than 2 years in the future");

            RuleFor(x => x.Status)
                .IsInEnum()
                .WithMessage("Status must be a valid match status");

            RuleFor(x => x)
                .Must(x => x.HomeTeamId != x.AwayTeamId)
                .WithMessage("Home team and away team cannot be the same")
                .WithName("Teams");
        }
    }
}
