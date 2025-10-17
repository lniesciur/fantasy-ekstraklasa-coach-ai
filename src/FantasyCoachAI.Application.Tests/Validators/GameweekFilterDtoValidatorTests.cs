using FantasyCoachAI.Application.DTOs;
using FantasyCoachAI.Application.Validators;
using FantasyCoachAI.Domain.Enums;
using FluentValidation.TestHelper;

namespace FantasyCoachAI.Application.Tests.Validators
{
    public class GameweekFilterDtoValidatorTests
    {
        private readonly GameweekFilterDtoValidator _validator;

        public GameweekFilterDtoValidatorTests()
        {
            _validator = new GameweekFilterDtoValidator();
        }

        [Fact]
        public void Validate_WhenSortIsValid_ShouldNotHaveValidationError()
        {
            // Arrange
            var filter = new GameweekFilterDto { Sort = "number" };

            // Act & Assert
            _validator.TestValidate(filter).ShouldNotHaveValidationErrorFor(x => x.Sort);
        }

        [Fact]
        public void Validate_WhenSortIsInvalid_ShouldHaveValidationError()
        {
            // Arrange
            var filter = new GameweekFilterDto { Sort = "invalid_sort" };

            // Act & Assert
            _validator.TestValidate(filter).ShouldHaveValidationErrorFor(x => x.Sort);
        }

        [Fact]
        public void Validate_WhenOrderIsValid_ShouldNotHaveValidationError()
        {
            // Arrange
            var filter = new GameweekFilterDto { Order = "asc" };

            // Act & Assert
            _validator.TestValidate(filter).ShouldNotHaveValidationErrorFor(x => x.Order);
        }

        [Fact]
        public void Validate_WhenOrderIsInvalid_ShouldHaveValidationError()
        {
            // Arrange
            var filter = new GameweekFilterDto { Order = "invalid_order" };

            // Act & Assert
            _validator.TestValidate(filter).ShouldHaveValidationErrorFor(x => x.Order);
        }

        [Fact]
        public void Validate_WhenStatusIsValid_ShouldNotHaveValidationError()
        {
            // Arrange
            var filter = new GameweekFilterDto { Status = GameweekStatus.Current };

            // Act & Assert
            _validator.TestValidate(filter).ShouldNotHaveValidationErrorFor(x => x.Status);
        }

        [Fact]
        public void Validate_WhenStatusIsInvalid_ShouldHaveValidationError()
        {
            // Arrange
            var filter = new GameweekFilterDto { Status = (GameweekStatus)999 };

            // Act & Assert
            _validator.TestValidate(filter).ShouldHaveValidationErrorFor(x => x.Status);
        }
    }
}
