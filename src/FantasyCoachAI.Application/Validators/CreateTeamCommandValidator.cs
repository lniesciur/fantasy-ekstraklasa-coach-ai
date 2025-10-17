using FantasyCoachAI.Application.DTOs;
using FluentValidation;

namespace FantasyCoachAI.Application.Validators
{
    public class CreateTeamCommandValidator : AbstractValidator<CreateTeamCommand>
    {
        public CreateTeamCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Team name is required")
                .MaximumLength(100)
                .WithMessage("Team name cannot exceed 100 characters");

            RuleFor(x => x.ShortCode)
                .NotEmpty()
                .WithMessage("Short code is required")
                .MaximumLength(10)
                .WithMessage("Short code cannot exceed 10 characters")
                .Matches(@"^[A-Z]{2,3}$")
                .WithMessage("Short code must be 2-3 uppercase letters");

            RuleFor(x => x.CrestUrl)
                .MaximumLength(500)
                .WithMessage("Crest URL cannot exceed 500 characters")
                .Must(BeValidUrl)
                .WithMessage("Crest URL must be a valid URL")
                .When(x => !string.IsNullOrEmpty(x.CrestUrl));

            RuleFor(x => x.IsActive)
                .NotNull()
                .WithMessage("IsActive field is required");
        }

        private bool BeValidUrl(string? url)
        {
            if (string.IsNullOrEmpty(url))
                return true;

            return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }
    }
}
