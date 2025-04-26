using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GamePlatform.Data;
using GamePlatform.Models;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace GamePlatform.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AdminController> _logger;

    public AdminController(ApplicationDbContext context, ILogger<AdminController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet("users")]
    public async Task<ActionResult<IEnumerable<object>>> GetUsers()
    {
        try
        {
            var users = await _context.Users
                .Select(u => new
                {
                    u.Id,
                    u.Username,
                    u.Email,
                    u.Role,
                    u.CreatedAt
                })
                .ToListAsync();

            return Ok(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting users");
            return StatusCode(500, new { message = "An error occurred while retrieving users" });
        }
    }

    [HttpGet("users/{id}")]
    public async Task<ActionResult<object>> GetUser(int id)
    {
        try
        {
            var user = await _context.Users
                .Select(u => new
                {
                    u.Id,
                    u.Username,
                    u.Email,
                    u.Role,
                    u.CreatedAt
                })
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user with ID {Id}", id);
            return StatusCode(500, new { message = "An error occurred while retrieving the user" });
        }
    }

    [HttpPost("users")]
    public async Task<ActionResult<object>> CreateUser([FromBody] UserCreateRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid user data", errors = ModelState.Values.SelectMany(v => v.Errors) });
            }

            // Validate username
            if (string.IsNullOrWhiteSpace(request.Username))
            {
                return BadRequest(new { message = "Username is required" });
            }

            if (await _context.Users.AnyAsync(u => u.Username == request.Username))
            {
                return BadRequest(new { message = "Username already exists" });
            }

            // Validate email
            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return BadRequest(new { message = "Email is required" });
            }

            if (!Regex.IsMatch(request.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                return BadRequest(new { message = "Invalid email format" });
            }

            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            {
                return BadRequest(new { message = "Email already exists" });
            }

            // Validate password
            if (string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new { message = "Password is required" });
            }

            if (request.Password.Length < 6)
            {
                return BadRequest(new { message = "Password must be at least 6 characters long" });
            }

            // Validate role
            if (string.IsNullOrWhiteSpace(request.Role))
            {
                return BadRequest(new { message = "Role is required" });
            }

            if (request.Role != "Admin" && request.Role != "User")
            {
                return BadRequest(new { message = "Invalid role" });
            }

            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = request.Role,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var createdUser = new
            {
                user.Id,
                user.Username,
                user.Email,
                user.Role,
                user.CreatedAt
            };

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, createdUser);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user");
            return StatusCode(500, new { message = "An error occurred while creating the user" });
        }
    }

    [HttpPut("users/{id}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UserUpdateRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid user data", errors = ModelState.Values.SelectMany(v => v.Errors) });
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            // Validate email if changed
            if (request.Email != null && request.Email != user.Email)
            {
                if (!Regex.IsMatch(request.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                {
                    return BadRequest(new { message = "Invalid email format" });
                }

                if (await _context.Users.AnyAsync(u => u.Email == request.Email && u.Id != id))
                {
                    return BadRequest(new { message = "Email already exists" });
                }

                user.Email = request.Email;
            }

            // Validate password if changed
            if (!string.IsNullOrWhiteSpace(request.Password))
            {
                if (request.Password.Length < 6)
                {
                    return BadRequest(new { message = "Password must be at least 6 characters long" });
                }

                user.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);
            }

            // Validate role if changed
            if (request.Role != null)
            {
                if (request.Role != "Admin" && request.Role != "User")
                {
                    return BadRequest(new { message = "Invalid role" });
                }

                user.Role = request.Role;
            }

            await _context.SaveChangesAsync();

            var updatedUser = new
            {
                user.Id,
                user.Username,
                user.Email,
                user.Role,
                user.CreatedAt
            };

            return Ok(new { message = "User updated successfully", user = updatedUser });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user with ID {Id}", id);
            return StatusCode(500, new { message = "An error occurred while updating the user" });
        }
    }

    [HttpPut("users/{id}/role")]
    public async Task<IActionResult> UpdateUserRole(int id, [FromBody] string role)
    {
        try
        {
            if (string.IsNullOrEmpty(role))
            {
                return BadRequest(new { message = "Role cannot be empty" });
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            user.Role = role;
            await _context.SaveChangesAsync();

            return Ok(new { message = "User role updated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user role for user ID {Id}", id);
            return StatusCode(500, new { message = "An error occurred while updating the user role" });
        }
    }

    [HttpDelete("users/{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        try
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user with ID {Id}", id);
            return StatusCode(500, new { message = "An error occurred while deleting the user" });
        }
    }
}

public class UserCreateRequest
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }
}

public class UserUpdateRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }
} 