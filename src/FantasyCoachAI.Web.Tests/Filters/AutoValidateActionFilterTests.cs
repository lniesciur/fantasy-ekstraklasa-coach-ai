using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using FantasyCoachAI.Application.DTOs;
using FantasyCoachAI.Web.Filters;
using FluentAssertions;
using FluentValidation;
using Moq;
using Xunit;
using System.Text.Json;

namespace FantasyCoachAI.Web.Tests.Filters
{
    [Trait("Category", "Unit")]
    public class AutoValidateActionFilterTests
    {
        private readonly Mock<ILogger<AutoValidateActionFilter>> _mockLogger;
        private readonly AutoValidateActionFilter _filter;

        public AutoValidateActionFilterTests()
        {
            _mockLogger = new Mock<ILogger<AutoValidateActionFilter>>();
            _filter = new AutoValidateActionFilter(_mockLogger.Object);
        }

        #region OnActionExecuting Tests

        [Fact]
        public void OnActionExecuting_WhenNoValidatorsFound_ShouldNotSetResult()
        {
            // Arrange
            var context = CreateActionExecutingContext();
            var testDto = new TestDto { Name = "Test" };

            // Mock service provider to return null validator
            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(x => x.GetService(It.IsAny<Type>()))
                .Returns(null);
            context.HttpContext.RequestServices = serviceProviderMock.Object;

            // Add parameter to context
            context.ActionArguments.Add("testParam", testDto);

            // Act
            _filter.OnActionExecuting(context);

            // Assert
            context.Result.Should().BeNull();
        }

        [Fact]
        public void OnActionExecuting_WhenValidationSucceeds_ShouldNotSetResultAndLogDebug()
        {
            // Arrange
            var context = CreateActionExecutingContext();
            var testDto = new TestDto { Name = "Valid Name" };

            // Create a mock validator that returns valid result
            var mockValidator = new Mock<IValidator<TestDto>>();
            var validResult = new FluentValidation.Results.ValidationResult();
            mockValidator.Setup(x => x.Validate(testDto)).Returns(validResult);

            // Mock service provider to return the validator
            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(x => x.GetService(typeof(IValidator<TestDto>)))
                .Returns(mockValidator.Object);
            context.HttpContext.RequestServices = serviceProviderMock.Object;

            // Add parameter to context
            context.ActionArguments.Add("testParam", testDto);

            // Act
            _filter.OnActionExecuting(context);

            // Assert
            context.Result.Should().BeNull();
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Debug,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Validation passed for")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public void OnActionExecuting_WhenValidationFails_ShouldSetBadRequestResultAndLogWarning()
        {
            // Arrange
            var context = CreateActionExecutingContext();
            var testDto = new TestDto { Name = "" }; // Invalid - empty name

            // Create validation failures
            var failures = new List<FluentValidation.Results.ValidationFailure>
            {
                new("Name", "Name is required") { AttemptedValue = "" }
            };
            var invalidResult = new FluentValidation.Results.ValidationResult(failures);

            // Create a mock validator that returns invalid result
            var mockValidator = new Mock<IValidator<TestDto>>();
            mockValidator.Setup(x => x.Validate(testDto)).Returns(invalidResult);

            // Mock service provider to return the validator
            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(x => x.GetService(typeof(IValidator<TestDto>)))
                .Returns(mockValidator.Object);
            context.HttpContext.RequestServices = serviceProviderMock.Object;

            // Add parameter to context
            context.ActionArguments.Add("testParam", testDto);

            // Act
            _filter.OnActionExecuting(context);

            // Assert
            context.Result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = (BadRequestObjectResult)context.Result;
            var errorResponse = badRequestResult.Value.Should().BeOfType<dynamic>().Subject;

            ((string)errorResponse.message).Should().Be("Validation failed");
            ((IEnumerable<dynamic>)errorResponse.errors).Should().HaveCount(1);

            var error = ((IEnumerable<dynamic>)errorResponse.errors).First();
            ((string)error.field).Should().Be("Name");
            ((string)error.message).Should().Be("Name is required");
            ((object)error.attemptedValue).Should().Be("");

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Validation failed for")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public void OnActionExecuting_WhenMultipleValidationErrors_ShouldReturnAllErrors()
        {
            // Arrange
            var context = CreateActionExecutingContext();
            var testDto = new TestDto { Name = "", Age = -5 };

            // Create multiple validation failures
            var failures = new List<FluentValidation.Results.ValidationFailure>
            {
                new("Name", "Name is required") { AttemptedValue = "" },
                new("Age", "Age must be positive") { AttemptedValue = -5 }
            };
            var invalidResult = new FluentValidation.Results.ValidationResult(failures);

            // Create a mock validator
            var mockValidator = new Mock<IValidator<TestDto>>();
            mockValidator.Setup(x => x.Validate(testDto)).Returns(invalidResult);

            // Mock service provider
            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(x => x.GetService(typeof(IValidator<TestDto>)))
                .Returns(mockValidator.Object);
            context.HttpContext.RequestServices = serviceProviderMock.Object;

            // Add parameter to context
            context.ActionArguments.Add("testParam", testDto);

            // Act
            _filter.OnActionExecuting(context);

            // Assert
            var badRequestResult = (BadRequestObjectResult)context.Result;
            var errorResponse = badRequestResult.Value.Should().BeOfType<dynamic>().Subject;

            ((IEnumerable<dynamic>)errorResponse.errors).Should().HaveCount(2);
        }

        [Fact]
        public void OnActionExecuting_WhenArgumentIsNull_ShouldSkipValidation()
        {
            // Arrange
            var context = CreateActionExecutingContext();

            // Mock service provider
            var serviceProviderMock = new Mock<IServiceProvider>();
            context.HttpContext.RequestServices = serviceProviderMock.Object;

            // Add null parameter to context
            context.ActionArguments.Add("testParam", null);

            // Act
            _filter.OnActionExecuting(context);

            // Assert
            context.Result.Should().BeNull();
            serviceProviderMock.Verify(x => x.GetService(It.IsAny<Type>()), Times.Never);
        }

        [Fact]
        public void OnActionExecuting_WhenValidatorThrowsException_ShouldNotSetResult()
        {
            // Arrange
            var context = CreateActionExecutingContext();
            var testDto = new TestDto { Name = "Test" };

            // Create a mock validator that throws exception
            var mockValidator = new Mock<IValidator<TestDto>>();
            mockValidator.Setup(x => x.Validate(testDto)).Throws(new Exception("Validator error"));

            // Mock service provider
            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(x => x.GetService(typeof(IValidator<TestDto>)))
                .Returns(mockValidator.Object);
            context.HttpContext.RequestServices = serviceProviderMock.Object;

            // Add parameter to context
            context.ActionArguments.Add("testParam", testDto);

            // Act & Assert
            Assert.Throws<Exception>(() => _filter.OnActionExecuting(context));
        }

        #endregion

        #region OnActionExecuted Tests

        [Fact]
        public void OnActionExecuted_ShouldDoNothing()
        {
            // Arrange
            var context = new ActionExecutedContext(
                new ActionContext(),
                new List<IFilterMetadata>(),
                new Mock<Controller>().Object);

            // Act
            _filter.OnActionExecuted(context);

            // Assert
            // No assertions needed - method should do nothing
        }

        #endregion

        #region Helper Methods

        private ActionExecutingContext CreateActionExecutingContext()
        {
            var actionContext = new ActionContext
            {
                HttpContext = new DefaultHttpContext()
            };

            var actionDescriptor = new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor
            {
                Parameters = new List<Microsoft.AspNetCore.Mvc.Abstractions.ParameterDescriptor>()
            };

            return new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                new Mock<Controller>().Object)
            {
                ActionDescriptor = actionDescriptor
            };
        }

        #endregion

        #region Test Classes

        public class TestDto
        {
            public string Name { get; set; } = string.Empty;
            public int Age { get; set; }
        }

        #endregion
    }
}
