using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GamePlatform.DTOs;
using GamePlatform.Services.Admin;

namespace GamePlatform.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Route("api/admin/games")]
    [ApiController]
    public class AdminGameController : ControllerBase
    {
        private readonly IAdminGameService _gameService;

        public AdminGameController(IAdminGameService gameService)
        {
            _gameService = gameService;
        }

        [HttpGet]
        public async Task<ActionResult<List<GameDTO>>> GetAllGames()
        {
            var games = await _gameService.GetAllGamesAsync();
            return Ok(games);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GameDTO>> GetGame(int id)
        {
            var game = await _gameService.GetGameByIdAsync(id);
            if (game == null)
                return NotFound();
            return Ok(game);
        }

        [HttpPost]
        public async Task<ActionResult<GameDTO>> CreateGame(GameDTO game)
        {
            var createdGame = await _gameService.CreateGameAsync(game);
            return CreatedAtAction(nameof(GetGame), new { id = createdGame.Id }, createdGame);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGame(int id, GameDTO game)
        {
            try
            {
                await _gameService.UpdateGameAsync(id, game);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGame(int id)
        {
            try
            {
                await _gameService.DeleteGameAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost("{id}/toggle-status")]
        public async Task<IActionResult> ToggleGameStatus(int id)
        {
            try
            {
                await _gameService.ToggleGameStatusAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
} 