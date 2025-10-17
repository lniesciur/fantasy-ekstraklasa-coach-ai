using FantasyCoachAI.Application.DTOs;
using FantasyCoachAI.Domain.Enums;
using FluentValidation;

namespace FantasyCoachAI.Application.Validators
{
    public class MatchFilterDtoValidator : AbstractValidator<MatchFilterDto>
    {
        private static readonly string[] ValidSortFields = { "match_date", "gameweek_number" };
        private static readonly string[] ValidOrderValues = { "asc", "desc" };

        public MatchFilterDtoValidator()
        {
            RuleFor(x => x.GameweekId)
                .GreaterThan(0)
                .WithMessage("GameweekId must be greater than zero")
                .When(x => x.GameweekId.HasValue);

            RuleFor(x => x.TeamId)
                .GreaterThan(0)
                .WithMessage("TeamId must be greater than zero")
                .When(x => x.TeamId.HasValue);

            RuleFor(x => x.Status)
                .IsInEnum()
                .WithMessage("Status must be a valid match status")
                .When(x => x.Status.HasValue);

            RuleFor(x => x.DateFrom)
                .LessThanOrEqualTo(x => x.DateTo)
                .WithMessage("DateFrom must be less than or equal to DateTo")
                .When(x => x.DateFrom.HasValue && x.DateTo.HasValue);

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
                .LessThanOrEqualTo(100)
                .WithMessage("Limit must be between 1 and 100");
        }
    }
}