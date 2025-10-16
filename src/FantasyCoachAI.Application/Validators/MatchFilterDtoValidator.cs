using FantasyCoachAI.Application.DTOs;
using FluentValidation;

namespace FantasyCoachAI.Application.Validators
{
    public class MatchFilterDtoValidator : AbstractValidator<MatchFilterDto>
    {
        public MatchFilterDtoValidator()
        {
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
