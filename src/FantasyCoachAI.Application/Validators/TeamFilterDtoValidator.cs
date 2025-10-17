using FantasyCoachAI.Application.DTOs;
using FluentValidation;

namespace FantasyCoachAI.Application.Validators
{
    public class TeamFilterDtoValidator : AbstractValidator<TeamFilterDto>
    {
        private static readonly string[] ValidSortFields = { "name", "shortcode" };
        private static readonly string[] ValidOrderValues = { "asc", "desc" };

        public TeamFilterDtoValidator()
        {
            RuleFor(x => x.Sort)
                .Must(sort => string.IsNullOrEmpty(sort) || ValidSortFields.Contains(sort.ToLower()))
                .WithMessage($"Sort must be one of: {string.Join(", ", ValidSortFields)}");

            RuleFor(x => x.Order)
                .Must(order => string.IsNullOrEmpty(order) || ValidOrderValues.Contains(order.ToLower()))
                .WithMessage($"Order must be one of: {string.Join(", ", ValidOrderValues)}");

            RuleFor(x => x.ShortCode)
                .MaximumLength(10)
                .WithMessage("ShortCode cannot exceed 10 characters")
                .When(x => !string.IsNullOrEmpty(x.ShortCode));
        }
    }
}
