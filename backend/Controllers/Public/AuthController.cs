using Microsoft.AspNetCore.Mvc;
using GamePlatform.Models;
using GamePlatform.Services;
using Microsoft.Extensions.Logging;

namespace GamePlatform.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(AuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        try
        {
            _logger.LogInformation($"Login request received for user: {request.Username}");
            
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            {
                _logger.LogWarning("Login failed: Invalid input data");
                return BadRequest(new { message = "Username and password are required" });
            }

            var response = await _authService.Login(request);
            if (response == null)
            {
                _logger.LogWarning($"Login failed for user: {request.Username}");
                return Unauthorized(new { message = "Invalid username or password" });
            }

            _logger.LogInformation($"Login successful for user: {request.Username}");
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error during login for user: {request.Username}");
            return StatusCode(500, new { message = "An error occurred during login" });
        }
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
    {
        try
        {
            _logger.LogInformation($"Registration request received for user: {request.Username}");

            if (string.IsNullOrWhiteSpace(request.Username) || 
                string.IsNullOrWhiteSpace(request.Email) || 
                string.IsNullOrWhiteSpace(request.Password))
            {
                _logger.LogWarning("Registration failed: Invalid input data");
                return BadRequest(new { message = "All fields are required" });
            }

            if (request.Password.Length < 6)
            {
                _logger.LogWarning("Registration failed: Password too short");
                return BadRequest(new { message = "Password must be at least 6 characters long" });
            }

            try
            {
                var response = await _authService.Register(request);
                _logger.LogInformation($"Registration successful for user: {request.Username}");
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning($"Registration failed: {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error during registration for user: {request.Username}");
            return StatusCode(500, new { message = "An error occurred during registration" });
        }
    }
} 