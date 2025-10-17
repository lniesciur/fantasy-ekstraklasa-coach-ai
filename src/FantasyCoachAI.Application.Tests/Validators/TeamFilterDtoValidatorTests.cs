using FantasyCoachAI.Application.DTOs;
using FantasyCoachAI.Application.Validators;
using FluentValidation.TestHelper;

namespace FantasyCoachAI.Application.Tests.Validators
{
    public class TeamFilterDtoValidatorTests
    {
        private readonly TeamFilterDtoValidator _validator;

        public TeamFilterDtoValidatorTests()
        {
            _validator = new TeamFilterDtoValidator();
        }

        [Fact]
        public void Validate_WhenSortIsValid_ShouldNotHaveValidationError()
        {
            // Arrange
            var filter = new TeamFilterDto { Sort = "name" };

            // Act & Assert
            _validator.TestValidate(filter).ShouldNotHaveValidationErrorFor(x => x.Sort);
        }

        [Fact]
        public void Validate_WhenSortIsInvalid_ShouldHaveValidationError()
        {
            // Arrange
            var filter = new TeamFilterDto { Sort = "invalid_sort" };

            // Act & Assert
            _validator.TestValidate(filter).ShouldHaveValidationErrorFor(x => x.Sort);
        }

        [Fact]
        public void Validate_WhenShortCodeIsTooLong_ShouldHaveValidationError()
        {
            // Arrange
            var filter = new TeamFilterDto { ShortCode = "VERYLONGCODE" };

            // Act & Assert
            _validator.TestValidate(filter).ShouldHaveValidationErrorFor(x => x.ShortCode);
        }

        [Fact]
        public void Validate_WhenShortCodeIsValid_ShouldNotHaveValidationError()
        {
            // Arrange
            var filter = new TeamFilterDto { ShortCode = "LEG" };

            // Act & Assert
            _validator.TestValidate(filter).ShouldNotHaveValidationErrorFor(x => x.ShortCode);
        }
    }
}
