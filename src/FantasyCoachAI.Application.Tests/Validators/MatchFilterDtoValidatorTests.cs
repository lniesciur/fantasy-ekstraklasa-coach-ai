using FantasyCoachAI.Application.DTOs;
using FantasyCoachAI.Application.Validators;
using FantasyCoachAI.Domain.Enums;
using FluentValidation.TestHelper;

namespace FantasyCoachAI.Application.Tests.Validators
{
    public class MatchFilterDtoValidatorTests
    {
        private readonly MatchFilterDtoValidator _validator;

        public MatchFilterDtoValidatorTests()
        {
            _validator = new MatchFilterDtoValidator();
        }

        [Fact]
        public void Validate_WhenGameweekIdIsValid_ShouldNotHaveValidationError()
        {
            // Arrange
            var filter = new MatchFilterDto { GameweekId = 1 };

            // Act & Assert
            _validator.TestValidate(filter).ShouldNotHaveValidationErrorFor(x => x.GameweekId);
        }

        [Fact]
        public void Validate_WhenGameweekIdIsInvalid_ShouldHaveValidationError()
        {
            // Arrange
            var filter = new MatchFilterDto { GameweekId = 0 };

            // Act & Assert
            _validator.TestValidate(filter).ShouldHaveValidationErrorFor(x => x.GameweekId);
        }

        [Fact]
        public void Validate_WhenSortIsValid_ShouldNotHaveValidationError()
        {
            // Arrange
            var filter = new MatchFilterDto { Sort = "match_date" };

            // Act & Assert
            _validator.TestValidate(filter).ShouldNotHaveValidationErrorFor(x => x.Sort);
        }

        [Fact]
        public void Validate_WhenSortIsInvalid_ShouldHaveValidationError()
        {
            // Arrange
            var filter = new MatchFilterDto { Sort = "invalid_sort" };

            // Act & Assert
            _validator.TestValidate(filter).ShouldHaveValidationErrorFor(x => x.Sort);
        }

        [Fact]
        public void Validate_WhenLimitIsTooHigh_ShouldHaveValidationError()
        {
            // Arrange
            var filter = new MatchFilterDto { Limit = 101 };

            // Act & Assert
            _validator.TestValidate(filter).ShouldHaveValidationErrorFor(x => x.Limit);
        }

        [Fact]
        public void Validate_WhenLimitIsValid_ShouldNotHaveValidationError()
        {
            // Arrange
            var filter = new MatchFilterDto { Limit = 50 };

            // Act & Assert
            _validator.TestValidate(filter).ShouldNotHaveValidationErrorFor(x => x.Limit);
        }
    }
}
