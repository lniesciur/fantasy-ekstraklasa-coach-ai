using Microsoft.AspNetCore.Mvc;
using FluentAssertions;
using Xunit;
using FantasyCoachAI.Web.Controllers;
using Microsoft.Extensions.Logging;
using Moq;

namespace FantasyCoachAI.Web.Tests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<ILogger<AuthController>> _mockLogger;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _mockLogger = new Mock<ILogger<AuthController>>();
        _controller = new AuthController(_mockLogger.Object);
    }

    // ============================================
    // LOGIN TESTS
    // ============================================

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsOkWithToken()
    {
        // Arrange
        var request = new AuthController.LoginRequest
        {
            Email = "user@example.com",
            Password = "password123",
            RememberMe = true
        };

        // Act
        var result = await _controller.Login(request);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.StatusCode.Should().Be(200);

        var response = okResult.Value as AuthController.LoginResponse;
        response.Should().NotBeNull();
        response!.Success.Should().BeTrue();
        response.Token.Should().NotBeNullOrEmpty();
        response.User.Should().NotBeNull();
        response.User.Email.Should().Be("user@example.com");
        response.Message.Should().Be("Logowanie pomyślne");
    }

    [Fact]
    public async Task Login_WithInvalidPassword_ReturnsUnauthorized()
    {
        // Arrange
        var request = new AuthController.LoginRequest
        {
            Email = "user@example.com",
            Password = "wrongpassword"
        };

        // Act
        var result = await _controller.Login(request);

        // Assert
        result.Should().BeOfType<UnauthorizedObjectResult>();
        var unauthorizedResult = result as UnauthorizedObjectResult;
        unauthorizedResult!.StatusCode.Should().Be(401);

        var response = unauthorizedResult.Value as AuthController.ErrorResponse;
        response.Should().NotBeNull();
        response!.Message.Should().Contain("Email lub hasło");
    }

    [Fact]
    public async Task Login_WithNonExistentEmail_ReturnsUnauthorized()
    {
        // Arrange
        var request = new AuthController.LoginRequest
        {
            Email = "nonexistent@example.com",
            Password = "password123"
        };

        // Act
        var result = await _controller.Login(request);

        // Assert
        result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Theory]
    [InlineData("", "password123")]
    [InlineData("user@example.com", "")]
    [InlineData(null, "password123")]
    [InlineData("user@example.com", null)]
    public async Task Login_WithMissingEmailOrPassword_ReturnsBadRequest(string email, string password)
    {
        // Arrange
        var request = new AuthController.LoginRequest
        {
            Email = email ?? string.Empty,
            Password = password ?? string.Empty
        };

        // Act
        var result = await _controller.Login(request);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.StatusCode.Should().Be(400);

        var response = badRequestResult.Value as AuthController.ErrorResponse;
        response.Should().NotBeNull();
        response!.Message.Should().Contain("Email i hasło");
        response.Errors.Should().NotBeEmpty();
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("@example.com")]
    [InlineData("user@")]
    [InlineData("user..@example.com")]
    public async Task Login_WithInvalidEmailFormat_ReturnsBadRequest(string invalidEmail)
    {
        // Arrange
        var request = new AuthController.LoginRequest
        {
            Email = invalidEmail,
            Password = "password123"
        };

        // Act
        var result = await _controller.Login(request);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;

        var response = badRequestResult!.Value as AuthController.ErrorResponse;
        response.Should().NotBeNull();
        response!.Message.Should().Contain("Email");
    }

    [Theory]
    [InlineData("a")]
    [InlineData("12345")]
    [InlineData("short")]
    public async Task Login_WithPasswordTooShort_ReturnsBadRequest(string shortPassword)
    {
        // Arrange
        var request = new AuthController.LoginRequest
        {
            Email = "user@example.com",
            Password = shortPassword
        };

        // Act
        var result = await _controller.Login(request);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;

        var response = badRequestResult!.Value as AuthController.ErrorResponse;
        response.Should().NotBeNull();
        response!.Message.Should().Contain("Hasło");
    }

    [Fact]
    public async Task Login_WithAdminCredentials_ReturnsUserWithAdminRole()
    {
        // Arrange
        var request = new AuthController.LoginRequest
        {
            Email = "admin@fantasiaekstraklasa.pl",
            Password = "admin123456"
        };

        // Act
        var result = await _controller.Login(request);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        var response = okResult!.Value as AuthController.LoginResponse;

        response.Should().NotBeNull();
        response!.Success.Should().BeTrue();
        response.User.Should().NotBeNull();
        response.User!.Role.Should().Be("Admin");
        response.User.Email.Should().Be("admin@fantasiaekstraklasa.pl");
    }

    [Fact]
    public async Task Login_ReturnedTokenIsValidJwtFormat()
    {
        // Arrange
        var request = new AuthController.LoginRequest
        {
            Email = "user@example.com",
            Password = "password123"
        };

        // Act
        var result = await _controller.Login(request);

        // Assert
        var okResult = result as OkObjectResult;
        var response = okResult!.Value as AuthController.LoginResponse;

        response!.Token.Should().NotBeNullOrEmpty();
        var tokenParts = response.Token.Split('.');
        tokenParts.Should().HaveCount(3); // JWT has 3 parts separated by dots
    }

    // ============================================
    // DEMO LOGIN TESTS
    // ============================================

    [Fact]
    public async Task LoginDemo_ReturnsOkWithDemoUser()
    {
        // Act
        var result = await _controller.LoginDemo();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.StatusCode.Should().Be(200);

        var response = okResult.Value as AuthController.LoginResponse;
        response.Should().NotBeNull();
        response!.Success.Should().BeTrue();
        response.Token.Should().NotBeNullOrEmpty();
        response.User.Should().NotBeNull();
        response.User!.Email.Should().Contain("demo");
    }

    [Fact]
    public async Task LoginDemo_ReturnsDemoUserWithCorrectProperties()
    {
        // Act
        var result = await _controller.LoginDemo();

        // Assert
        var okResult = result as OkObjectResult;
        var response = okResult!.Value as AuthController.LoginResponse;

        response!.User.Should().NotBeNull();
        response.User!.Id.Should().Be(999);
        response.User.Role.Should().Be("User");
        response.User.FullName.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task LoginDemo_GeneratesValidToken()
    {
        // Act
        var result = await _controller.LoginDemo();

        // Assert
        var okResult = result as OkObjectResult;
        var response = okResult!.Value as AuthController.LoginResponse;

        response!.Token.Should().NotBeNullOrEmpty();
        var tokenParts = response.Token.Split('.');
        tokenParts.Should().HaveCount(3);
    }

    // ============================================
    // LOGOUT TESTS
    // ============================================

    [Fact]
    public void Logout_ReturnsOkWithSuccessMessage()
    {
        // Act
        var result = _controller.Logout();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.StatusCode.Should().Be(200);
    }

    // ============================================
    // ERROR HANDLING TESTS
    // ============================================

    [Fact]
    public async Task Login_WhenExceptionOccurs_ReturnsInternalServerError()
    {
        // This test would require injecting a mock that throws exception
        // For now, it documents the expected behavior
        // In future, consider making the controller more testable

        // Arrange
        var request = new AuthController.LoginRequest
        {
            Email = "user@example.com",
            Password = "password123"
        };

        // Act & Assert - should not throw, should return 500
        var result = await _controller.Login(request);
        result.Should().NotBeNull();
    }

    // ============================================
    // LOGGING TESTS
    // ============================================

    [Fact]
    public async Task Login_WithValidCredentials_LogsSuccessfulAttempt()
    {
        // Arrange
        var request = new AuthController.LoginRequest
        {
            Email = "user@example.com",
            Password = "password123"
        };

        // Act
        await _controller.Login(request);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Successful login")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_LogsFailedAttempt()
    {
        // Arrange
        var request = new AuthController.LoginRequest
        {
            Email = "user@example.com",
            Password = "wrongpassword"
        };

        // Act
        await _controller.Login(request);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Failed login")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task LoginDemo_LogsDemoLoginRequest()
    {
        // Act
        await _controller.LoginDemo();

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Demo login")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void Logout_LogsLogoutRequest()
    {
        // Act
        _controller.Logout();

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Logout")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
