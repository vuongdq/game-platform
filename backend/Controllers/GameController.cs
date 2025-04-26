using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GamePlatform.Data;
using GamePlatform.Models;

namespace GamePlatform.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class GameController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<GameController> _logger;

    public GameController(ApplicationDbContext context, ILogger<GameController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetGames()
    {
        try
        {
            var games = await _context.Games
                .Include(g => g.Category)
                .Select(g => new
                {
                    g.Id,
                    g.Title,
                    g.Description,
                    g.ImageUrl,
                    g.GameUrl,
                    g.CategoryId,
                    Category = g.Category != null ? new
                    {
                        g.Category.Id,
                        g.Category.Name
                    } : null
                })
                .ToListAsync();

            return Ok(games);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting games");
            return StatusCode(500, new { message = "An error occurred while retrieving games" });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<object>> GetGame(int id)
    {
        try
        {
            var game = await _context.Games
                .Include(g => g.Category)
                .Select(g => new
                {
                    g.Id,
                    g.Title,
                    g.Description,
                    g.ImageUrl,
                    g.GameUrl,
                    g.CategoryId,
                    Category = g.Category != null ? new
                    {
                        g.Category.Id,
                        g.Category.Name
                    } : null
                })
                .FirstOrDefaultAsync(g => g.Id == id);

            if (game == null)
            {
                return NotFound(new { message = $"Game with ID {id} not found" });
            }

            return Ok(game);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting game with ID {Id}", id);
            return StatusCode(500, new { message = "An error occurred while retrieving the game" });
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<object>> CreateGame([FromBody] Game game)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid game data", errors = ModelState.Values.SelectMany(v => v.Errors) });
            }

            var category = await _context.Categories.FindAsync(game.CategoryId);
            if (category == null)
            {
                return BadRequest(new { message = "Invalid category ID" });
            }

            game.CreatedAt = DateTime.UtcNow;
            game.UpdatedAt = DateTime.UtcNow;
            
            _context.Games.Add(game);
            await _context.SaveChangesAsync();

            var createdGame = new
            {
                game.Id,
                game.Title,
                game.Description,
                game.ImageUrl,
                game.GameUrl,
                game.CategoryId,
                Category = new
                {
                    category.Id,
                    category.Name
                }
            };

            return CreatedAtAction(nameof(GetGame), new { id = game.Id }, createdGame);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating game");
            return StatusCode(500, new { message = "An error occurred while creating the game" });
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateGame(int id, [FromBody] GameUpdateRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid game data", errors = ModelState.Values.SelectMany(v => v.Errors) });
            }

            var existingGame = await _context.Games.FindAsync(id);
            if (existingGame == null)
            {
                return NotFound(new { message = $"Game with ID {id} not found" });
            }

            var category = await _context.Categories.FindAsync(request.CategoryId);
            if (category == null)
            {
                return BadRequest(new { message = "Invalid category ID" });
            }

            existingGame.Title = request.Title;
            existingGame.Description = request.Description;
            existingGame.ImageUrl = request.ImageUrl;
            existingGame.GameUrl = request.GameUrl;
            existingGame.CategoryId = request.CategoryId;
            existingGame.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { 
                    message = "Game updated successfully",
                    game = new {
                        existingGame.Id,
                        existingGame.Title,
                        existingGame.Description,
                        existingGame.ImageUrl,
                        existingGame.GameUrl,
                        existingGame.CategoryId,
                        Category = new {
                            category.Id,
                            category.Name
                        }
                    }
                });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GameExists(id))
                {
                    return NotFound(new { message = $"Game with ID {id} not found" });
                }
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating game with ID {Id}", id);
            return StatusCode(500, new { message = "An error occurred while updating the game" });
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGame(int id)
    {
        try
        {
            var game = await _context.Games.FindAsync(id);
            if (game == null)
            {
                return NotFound(new { message = $"Game with ID {id} not found" });
            }

            _context.Games.Remove(game);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Game deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting game with ID {Id}", id);
            return StatusCode(500, new { message = "An error occurred while deleting the game" });
        }
    }

    private bool GameExists(int id)
    {
        return _context.Games.Any(e => e.Id == id);
    }
}

public class GameUpdateRequest
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string ImageUrl { get; set; }
    public string GameUrl { get; set; }
    public int CategoryId { get; set; }
} 