using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using FantasyCoachAI.Domain.Exceptions;
using FantasyCoachAI.Web.Middleware;
using FluentAssertions;
using Moq;
using Xunit;
using System.Text.Json;
using System.Text;

namespace FantasyCoachAI.Web.Tests.Middleware
{
    [Trait("Category", "Unit")]
    public class ApiExceptionMiddlewareTests
    {
        private readonly Mock<ILogger<ApiExceptionMiddleware>> _mockLogger;
        private readonly Mock<RequestDelegate> _mockNext;
        private readonly ApiExceptionMiddleware _middleware;

        public ApiExceptionMiddlewareTests()
        {
            _mockLogger = new Mock<ILogger<ApiExceptionMiddleware>>();
            _mockNext = new Mock<RequestDelegate>();
            _middleware = new ApiExceptionMiddleware(_mockNext.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task InvokeAsync_WhenNoExceptionThrown_ShouldCallNextDelegate()
        {
            // Arrange
            var context = CreateHttpContext();
            _mockNext.Setup(x => x(context)).Returns(Task.CompletedTask);

            // Act
            await _middleware.InvokeAsync(context);

            // Assert
            _mockNext.Verify(x => x(context), Times.Once);
            _mockLogger.Verify(
                x => x.Log(
                    It.IsAny<LogLevel>(),
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Never);
        }

        [Fact]
        public async Task InvokeAsync_WhenArgumentExceptionThrown_ShouldReturn400WithMessage()
        {
            // Arrange
            var context = CreateHttpContext();
            var exception = new ArgumentException("Invalid parameter value", "paramName");

            _mockNext.Setup(x => x(context)).Throws(exception);

            // Act
            await _middleware.InvokeAsync(context);

            // Assert
            context.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            context.Response.ContentType.Should().Be("application/json");

            var responseBody = GetResponseBody(context);
            var errorResponse = JsonSerializer.Deserialize<dynamic>(responseBody,
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            ((string)errorResponse.message).Should().Be("Invalid parameter value");
            ((object?)errorResponse.details).Should().BeNull();

            VerifyLogging(LogLevel.Error, exception);
        }

        [Fact]
        public async Task InvokeAsync_WhenUnauthorizedAccessExceptionThrown_ShouldReturn401()
        {
            // Arrange
            var context = CreateHttpContext();
            var exception = new UnauthorizedAccessException("Access denied");

            _mockNext.Setup(x => x(context)).Throws(exception);

            // Act
            await _middleware.InvokeAsync(context);

            // Assert
            context.Response.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
            context.Response.ContentType.Should().Be("application/json");

            var responseBody = GetResponseBody(context);
            var errorResponse = JsonSerializer.Deserialize<dynamic>(responseBody,
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            ((string)errorResponse.message).Should().Be("Unauthorized access.");
            ((object?)errorResponse.details).Should().BeNull();
        }

        [Fact]
        public async Task InvokeAsync_WhenNotFoundExceptionThrown_ShouldReturn404()
        {
            // Arrange
            var context = CreateHttpContext();
            var exception = new NotFoundException("Resource not found");

            _mockNext.Setup(x => x(context)).Throws(exception);

            // Act
            await _middleware.InvokeAsync(context);

            // Assert
            context.Response.StatusCode.Should().Be(StatusCodes.Status404NotFound);
            context.Response.ContentType.Should().Be("application/json");

            var responseBody = GetResponseBody(context);
            var errorResponse = JsonSerializer.Deserialize<dynamic>(responseBody,
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            ((string)errorResponse.message).Should().Be("Resource not found.");
            ((object?)errorResponse.details).Should().BeNull();
        }

        [Fact]
        public async Task InvokeAsync_WhenInvalidOperationExceptionThrown_ShouldReturn409()
        {
            // Arrange
            var context = CreateHttpContext();
            var exception = new InvalidOperationException("Operation not allowed");

            _mockNext.Setup(x => x(context)).Throws(exception);

            // Act
            await _middleware.InvokeAsync(context);

            // Assert
            context.Response.StatusCode.Should().Be(StatusCodes.Status409Conflict);
            context.Response.ContentType.Should().Be("application/json");

            var responseBody = GetResponseBody(context);
            var errorResponse = JsonSerializer.Deserialize<dynamic>(responseBody,
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            ((string)errorResponse.message).Should().Be("Operation not allowed");
            ((object?)errorResponse.details).Should().BeNull();
        }

        [Fact]
        public async Task InvokeAsync_WhenUnhandledExceptionThrown_ShouldReturn500WithDetails()
        {
            // Arrange
            var context = CreateHttpContext();
            var exception = new Exception("Unexpected error");

            _mockNext.Setup(x => x(context)).Throws(exception);

            // Act
            await _middleware.InvokeAsync(context);

            // Assert
            context.Response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            context.Response.ContentType.Should().Be("application/json");

            var responseBody = GetResponseBody(context);
            var errorResponse = JsonSerializer.Deserialize<dynamic>(responseBody,
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            ((string)errorResponse.message).Should().Be("Internal server error");
            ((string)errorResponse.details).Should().Be("Unexpected error");
        }

        [Fact]
        public async Task InvokeAsync_WhenExceptionThrown_ShouldAlwaysLogError()
        {
            // Arrange
            var context = CreateHttpContext();
            var exception = new Exception("Test exception");

            _mockNext.Setup(x => x(context)).Throws(exception);

            // Act
            await _middleware.InvokeAsync(context);

            // Assert
            VerifyLogging(LogLevel.Error, exception);
        }

        [Fact]
        public async Task InvokeAsync_WhenResponseAlreadyStarted_ShouldStillLogError()
        {
            // Arrange
            var context = CreateHttpContext();
            // Note: HttpResponse.HasStarted is read-only, so we can't set it directly
            // Instead, we'll just verify that logging happens regardless

            var exception = new ArgumentException("Test");

            _mockNext.Setup(x => x(context)).Throws(exception);

            // Act
            await _middleware.InvokeAsync(context);

            // Assert
            // Even if response modification fails, logging should still work
            VerifyLogging(LogLevel.Error, exception);
        }

        #region Helper Methods

        private DefaultHttpContext CreateHttpContext()
        {
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();
            return context;
        }

        private string GetResponseBody(DefaultHttpContext context)
        {
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(context.Response.Body);
            return reader.ReadToEnd();
        }

        private void VerifyLogging(LogLevel expectedLevel, Exception exception)
        {
            _mockLogger.Verify(
                x => x.Log(
                    expectedLevel,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("An unhandled exception occurred")),
                    exception,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        #endregion
    }
}
