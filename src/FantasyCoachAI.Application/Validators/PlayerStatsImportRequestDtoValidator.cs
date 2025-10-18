using FantasyCoachAI.Application.DTOs;
using FluentValidation;

namespace FantasyCoachAI.Application.Validators
{
    public class PlayerStatsImportRequestDtoValidator : AbstractValidator<PlayerStatsImportRequestDto>
    {
        public PlayerStatsImportRequestDtoValidator()
        {
            RuleFor(x => x.File)
                .NotNull()
                .WithMessage("File is required")
                .Must(file => file != null && file.Length > 0)
                .WithMessage("File cannot be empty")
                .Must(file => file != null && file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                .WithMessage("Only CSV files are supported")
                .Must(file => file != null && file.Length <= 10 * 1024 * 1024) // 10 MB
                .WithMessage("File too large. Maximum size: 10MB");
        }
    }
}
