using FantasyCoachAI.Application.DTOs;
using FluentValidation;

namespace FantasyCoachAI.Application.Validators
{
    public class UpdateMatchCommandValidator : AbstractValidator<UpdateMatchCommand>
    {
        public UpdateMatchCommandValidator()
        {
            RuleFor(x => x.GameweekId)
                .GreaterThan(0)
                .WithMessage("GameweekId must be greater than zero")
                .When(x => x.GameweekId.HasValue);

            RuleFor(x => x.HomeTeamId)
                .GreaterThan(0)
                .WithMessage("HomeTeamId must be greater than zero")
                .When(x => x.HomeTeamId.HasValue);

            RuleFor(x => x.AwayTeamId)
                .GreaterThan(0)
                .WithMessage("AwayTeamId must be greater than zero")
                .When(x => x.AwayTeamId.HasValue);

            RuleFor(x => x.MatchDate)
                .GreaterThan(DateTime.UtcNow.AddDays(-1))
                .WithMessage("Match date cannot be more than 1 day in the past")
                .LessThan(DateTime.UtcNow.AddYears(2))
                .WithMessage("Match date cannot be more than 2 years in the future")
                .When(x => x.MatchDate.HasValue);

            RuleFor(x => x.Status)
                .IsInEnum()
                .WithMessage("Status must be a valid match status")
                .When(x => x.Status.HasValue);

            RuleFor(x => x.HomeScore)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Home score must be zero or greater")
                .When(x => x.HomeScore.HasValue);

            RuleFor(x => x.AwayScore)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Away score must be zero or greater")
                .When(x => x.AwayScore.HasValue);

            RuleFor(x => x.RescheduleReason)
                .MaximumLength(500)
                .WithMessage("Reschedule reason cannot exceed 500 characters")
                .When(x => !string.IsNullOrEmpty(x.RescheduleReason));

            // Custom rule: teams cannot be the same
            RuleFor(x => x)
                .Must(x => !x.HomeTeamId.HasValue || !x.AwayTeamId.HasValue || 
                          x.HomeTeamId.Value != x.AwayTeamId.Value)
                .WithMessage("Home team and away team cannot be the same")
                .WithName("Teams")
                .When(x => x.HomeTeamId.HasValue && x.AwayTeamId.HasValue);
        }
    }
}
