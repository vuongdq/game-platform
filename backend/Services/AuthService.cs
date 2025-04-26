using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GamePlatform.Data;
using GamePlatform.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Logging;

namespace GamePlatform.Services;

public class AuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;

    public AuthService(ApplicationDbContext context, IConfiguration configuration, ILogger<AuthService> logger)
    {
        _context = context;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<AuthResponse?> Login(LoginRequest request)
    {
        try
        {
            _logger.LogInformation($"Attempting login for user: {request.Username}");
            
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
            if (user == null)
            {
                _logger.LogWarning($"Login failed: User {request.Username} not found");
                return null;
            }

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                _logger.LogWarning($"Login failed: Invalid password for user {request.Username}");
                return null;
            }

            var token = GenerateJwtToken(user);
            _logger.LogInformation($"Login successful for user: {request.Username}");
            
            return new AuthResponse
            {
                Token = token,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error during login for user: {request.Username}");
            throw;
        }
    }

    public async Task<AuthResponse?> Register(RegisterRequest request)
    {
        try
        {
            _logger.LogInformation($"Starting registration process for user: {request.Username}");

            // Validate input
            if (string.IsNullOrWhiteSpace(request.Username))
            {
                _logger.LogWarning("Registration failed: Username is empty");
                throw new ArgumentException("Username is required");
            }

            if (string.IsNullOrWhiteSpace(request.Email))
            {
                _logger.LogWarning("Registration failed: Email is empty");
                throw new ArgumentException("Email is required");
            }

            if (string.IsNullOrWhiteSpace(request.Password))
            {
                _logger.LogWarning("Registration failed: Password is empty");
                throw new ArgumentException("Password is required");
            }

            if (request.Password.Length < 6)
            {
                _logger.LogWarning("Registration failed: Password is too short");
                throw new ArgumentException("Password must be at least 6 characters long");
            }

            // Check if username exists
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
            if (existingUser != null)
            {
                _logger.LogWarning($"Registration failed: Username {request.Username} already exists");
                throw new ArgumentException("Username already exists");
            }

            // Check if email exists
            existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (existingUser != null)
            {
                _logger.LogWarning($"Registration failed: Email {request.Email} already exists");
                throw new ArgumentException("Email already exists");
            }

            _logger.LogInformation("Creating new user...");
            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = "User",
                CreatedAt = DateTime.UtcNow
            };

            _logger.LogInformation("Adding user to database...");
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            _logger.LogInformation("User saved successfully");

            var token = GenerateJwtToken(user);
            _logger.LogInformation($"Registration successful for user: {request.Username}");

            return new AuthResponse
            {
                Token = token,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role
            };
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning($"Registration failed: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Unexpected error during registration for user: {request.Username}");
            throw new Exception("An unexpected error occurred during registration", ex);
        }
    }

    private string GenerateJwtToken(User user)
    {
        try
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is not configured")));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating JWT token");
            throw;
        }
    }
} 