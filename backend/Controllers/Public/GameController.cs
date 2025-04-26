using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GamePlatform.Data;
using GamePlatform.Models;
using GamePlatform.DTOs;
using GamePlatform.Services.Public;

namespace GamePlatform.Controllers.Public;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class GameController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<GameController> _logger;
    private readonly IGameService _gameService;

    public GameController(ApplicationDbContext context, ILogger<GameController> logger, IGameService gameService)
    {
        _context = context;
        _logger = logger;
        _gameService = gameService;
    }

    [HttpGet]
    public async Task<ActionResult<List<GameDTO>>> GetAllGames()
    {
        var games = await _gameService.GetAllGamesAsync();
        return Ok(games);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GameDTO>> GetGameById(int id)
    {
        var game = await _gameService.GetGameByIdAsync(id);
        if (game == null)
            return NotFound();
        return Ok(game);
    }

    [HttpGet("category/{categoryId}")]
    public async Task<ActionResult<List<GameDTO>>> GetGamesByCategory(int categoryId)
    {
        var games = await _gameService.GetGamesByCategoryAsync(categoryId);
        return Ok(games);
    }

    [HttpGet("popular/{count}")]
    public async Task<ActionResult<List<GameDTO>>> GetPopularGames(int count = 10)
    {
        var games = await _gameService.GetPopularGamesAsync(count);
        return Ok(games);
    }

    [HttpGet("recent/{count}")]
    public async Task<ActionResult<List<GameDTO>>> GetRecentlyPlayedGames(int count = 10)
    {
        var games = await _gameService.GetRecentlyPlayedGamesAsync(count);
        return Ok(games);
    }

    [HttpPost("{id}/play")]
    public async Task<ActionResult> UpdateGameStats(int id)
    {
        await _gameService.UpdateGameStatsAsync(id);
        return Ok();
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
                game.Name,
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

            return CreatedAtAction(nameof(GetGameById), new { id = game.Id }, createdGame);
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

            existingGame.Name = request.Name;
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
                        existingGame.Name,
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
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string GameUrl { get; set; } = string.Empty;
    public int CategoryId { get; set; }
} 