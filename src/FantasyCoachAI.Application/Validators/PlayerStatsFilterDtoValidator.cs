using FantasyCoachAI.Application.DTOs;
using FluentValidation;

namespace FantasyCoachAI.Application.Validators
{
    public class PlayerStatsFilterDtoValidator : AbstractValidator<PlayerStatsFilterDto>
    {
        private static readonly string[] ValidSortFields = { "fantasy_points", "form", "price", "minutes_played", "goals", "assists" };
        private static readonly string[] ValidOrderValues = { "asc", "desc" };
        private static readonly string[] ValidPositions = { "GK", "DEF", "MID", "FWD" };

        public PlayerStatsFilterDtoValidator()
        {
            RuleFor(x => x.TeamId)
                .GreaterThan(0)
                .WithMessage("TeamId must be greater than zero")
                .When(x => x.TeamId.HasValue);

            RuleFor(x => x.Position)
                .Must(position => string.IsNullOrEmpty(position) || ValidPositions.Contains(position.ToUpper()))
                .WithMessage($"Position must be one of: {string.Join(", ", ValidPositions)}")
                .When(x => !string.IsNullOrEmpty(x.Position));

            RuleFor(x => x.Sort)
                .Must(sort => string.IsNullOrEmpty(sort) || ValidSortFields.Contains(sort.ToLower()))
                .WithMessage($"Sort must be one of: {string.Join(", ", ValidSortFields)}");

            RuleFor(x => x.Order)
                .Must(order => string.IsNullOrEmpty(order) || ValidOrderValues.Contains(order.ToLower()))
                .WithMessage($"Order must be one of: {string.Join(", ", ValidOrderValues)}");

            RuleFor(x => x.Page)
                .GreaterThan(0)
                .WithMessage("Page must be greater than 0");

            RuleFor(x => x.Limit)
                .GreaterThan(0)
                .WithMessage("Limit must be greater than 0")
                .LessThanOrEqualTo(100)
                .WithMessage("Limit cannot exceed 100");
        }
    }
}
