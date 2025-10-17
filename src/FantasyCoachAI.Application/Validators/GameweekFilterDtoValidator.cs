using FantasyCoachAI.Application.DTOs;
using FantasyCoachAI.Domain.Enums;
using FluentValidation;

namespace FantasyCoachAI.Application.Validators
{
    public class GameweekFilterDtoValidator : AbstractValidator<GameweekFilterDto>
    {
        private static readonly string[] ValidSortFields = { "number", "start_date" };
        private static readonly string[] ValidOrderValues = { "asc", "desc" };

        public GameweekFilterDtoValidator()
        {
            RuleFor(x => x.Sort)
                .Must(sort => string.IsNullOrEmpty(sort) || ValidSortFields.Contains(sort.ToLower()))
                .WithMessage($"Sort must be one of: {string.Join(", ", ValidSortFields)}");

            RuleFor(x => x.Order)
                .Must(order => string.IsNullOrEmpty(order) || ValidOrderValues.Contains(order.ToLower()))
                .WithMessage($"Order must be one of: {string.Join(", ", ValidOrderValues)}");

            RuleFor(x => x.Status)
                .IsInEnum()
                .WithMessage("Status must be a valid gameweek status")
                .When(x => x.Status.HasValue);
        }
    }
}
