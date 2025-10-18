using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace FantasyCoachAI.Web.Controllers;

/// <summary>
/// Authentication controller handling user login and authentication
/// Currently mocked - TODO: Integrate with Supabase authentication
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;

    public AuthController(ILogger<AuthController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Login endpoint - handles user authentication
    /// TODO: Replace mock logic with real Supabase authentication
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            // Validation
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new ErrorResponse
                {
                    Message = "Email i hasło są wymagane.",
                    Errors = new Dictionary<string, string[]>
                    {
                        { "email", new[] { "Email jest wymagany" } },
                        { "password", new[] { "Hasło jest wymagane" } }
                    }
                });
            }

            // Email format validation
            if (!IsValidEmail(request.Email))
            {
                return BadRequest(new ErrorResponse
                {
                    Message = "Email nie ma prawidłowego formatu.",
                    Errors = new Dictionary<string, string[]>
                    {
                        { "email", new[] { "Email nie ma prawidłowego formatu" } }
                    }
                });
            }

            // Password validation
            if (request.Password.Length < 6)
            {
                return BadRequest(new ErrorResponse
                {
                    Message = "Hasło musi mieć co najmniej 6 znaków.",
                    Errors = new Dictionary<string, string[]>
                    {
                        { "password", new[] { "Hasło jest za krótkie" } }
                    }
                });
            }

            _logger.LogInformation($"Attempting login for user: {request.Email}");

            // ============================================
            // MOCK AUTHENTICATION - PLACEHOLDER
            // TODO: Replace with actual Supabase authentication
            // ============================================
            var mockUser = await MockAuthenticateUserAsync(request.Email, request.Password);

            if (mockUser == null)
            {
                _logger.LogWarning($"Failed login attempt for user: {request.Email}");
                return Unauthorized(new ErrorResponse
                {
                    Message = "Email lub hasło są nieprawidłowe."
                });
            }

            _logger.LogInformation($"Successful login for user: {request.Email}");

            // Generate mock token
            var mockToken = GenerateMockJwtToken(mockUser);

            return Ok(new LoginResponse
            {
                Success = true,
                Token = mockToken,
                Message = "Logowanie pomyślne",
                User = mockUser
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login process");
            return StatusCode(500, new ErrorResponse
            {
                Message = "Błąd serwera podczas logowania. Spróbuj ponownie później."
            });
        }
    }

    /// <summary>
    /// Demo login endpoint - allows testing without credentials
    /// Useful for development and testing
    /// </summary>
    [HttpPost("login-demo")]
    public async Task<IActionResult> LoginDemo()
    {
        try
        {
            _logger.LogInformation("Demo login requested");

            // Mock demo user
            var demoUser = new UserResponse
            {
                Id = 999,
                Email = "demo@fantasiaekstraklasa.pl",
                FullName = "Demo User",
                Role = "User"
            };

            var mockToken = GenerateMockJwtToken(demoUser);

            return Ok(new LoginResponse
            {
                Success = true,
                Token = mockToken,
                Message = "Demo login successful",
                User = demoUser
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during demo login");
            return StatusCode(500, new ErrorResponse
            {
                Message = "Błąd podczas logowania jako demo."
            });
        }
    }

    /// <summary>
    /// Logout endpoint - invalidates user session
    /// TODO: Implement session/token invalidation
    /// </summary>
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        try
        {
            _logger.LogInformation("Logout requested");
            // TODO: Implement proper logout logic (clear session, invalidate token)
            return Ok(new { message = "Wylogowanie pomyślne" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout");
            return StatusCode(500, new ErrorResponse
            {
                Message = "Błąd podczas wylogowywania."
            });
        }
    }

    // ============================================
    // MOCK/HELPER METHODS - TO BE REPLACED
    // ============================================

    /// <summary>
    /// Mock authentication - validates credentials against hardcoded list
    /// TODO: Replace with Supabase authentication
    /// </summary>
    private async Task<UserResponse?> MockAuthenticateUserAsync(string email, string password)
    {
        // Mock user database
        var mockUsers = new Dictionary<string, (string Password, UserResponse User)>
        {
            {
                "user@example.com",
                (
                    "password123",
                    new UserResponse
                    {
                        Id = 1,
                        Email = "user@example.com",
                        FullName = "Test User",
                        Role = "User"
                    }
                )
            },
            {
                "admin@fantasiaekstraklasa.pl",
                (
                    "admin123456",
                    new UserResponse
                    {
                        Id = 2,
                        Email = "admin@fantasiaekstraklasa.pl",
                        FullName = "Admin User",
                        Role = "Admin"
                    }
                )
            }
        };

        await Task.Delay(100); // Simulate network delay

        if (mockUsers.TryGetValue(email, out var userRecord))
        {
            if (userRecord.Password == password)
            {
                return userRecord.User;
            }
        }

        return null;
    }

    /// <summary>
    /// Generates a mock JWT token for testing
    /// TODO: Replace with real JWT generation from Supabase
    /// </summary>
    private string GenerateMockJwtToken(UserResponse user)
    {
        // Mock JWT token format: header.payload.signature
        var header = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(@"{""alg"":""HS256"",""typ"":""JWT""}"));
        var payload = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(
            $@"{{""sub"":""{user.Id}"",""email"":""{user.Email}"",""role"":""{user.Role}"",""exp"":{DateTimeOffset.UtcNow.AddHours(24).ToUnixTimeSeconds()}}}"));
        var signature = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("mock-signature"));

        return $"{header}.{payload}.{signature}";
    }

    /// <summary>
    /// Email validation helper
    /// </summary>
    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    // ============================================
    // DTOs
    // ============================================

    public class LoginRequest
    {
        [Required(ErrorMessage = "Email jest wymagany")]
        [EmailAddress(ErrorMessage = "Email nie ma prawidłowego formatu")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Hasło jest wymagane")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Hasło musi mieć co najmniej 6 znaków")]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; } = false;
    }

    public class LoginResponse
    {
        public bool Success { get; set; }
        public string? Token { get; set; }
        public string? Message { get; set; }
        public UserResponse? User { get; set; }
    }

    public class UserResponse
    {
        public int Id { get; set; }

        public string Email { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public string Role { get; set; } = "User";
    }

    public class ErrorResponse
    {
        public string? Message { get; set; }

        public Dictionary<string, string[]>? Errors { get; set; }
    }
}
