using GamePlatform.Data;
using GamePlatform.DTOs;
using GamePlatform.Models;
using Microsoft.EntityFrameworkCore;

namespace GamePlatform.Services.Admin
{
    public class AdminGameService : IAdminGameService
    {
        private readonly ApplicationDbContext _context;

        public AdminGameService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<GameDTO> CreateGameAsync(GameDTO game)
        {
            var newGame = new Game
            {
                Name = game.Name,
                Description = game.Description,
                ImageUrl = game.ImageUrl,
                GameUrl = game.GameUrl,
                CategoryId = game.CategoryId,
                IsActive = game.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            _context.Games.Add(newGame);
            await _context.SaveChangesAsync();

            return new GameDTO
            {
                Id = newGame.Id,
                Name = newGame.Name,
                Description = newGame.Description,
                ImageUrl = newGame.ImageUrl,
                GameUrl = newGame.GameUrl,
                CategoryId = newGame.CategoryId,
                IsActive = newGame.IsActive,
                CreatedAt = newGame.CreatedAt
            };
        }

        public async Task<GameDTO> UpdateGameAsync(int id, GameDTO game)
        {
            var existingGame = await _context.Games.FindAsync(id);
            if (existingGame == null)
                throw new KeyNotFoundException($"Game with ID {id} not found.");

            existingGame.Name = game.Name;
            existingGame.Description = game.Description;
            existingGame.ImageUrl = game.ImageUrl;
            existingGame.GameUrl = game.GameUrl;
            existingGame.CategoryId = game.CategoryId;
            existingGame.IsActive = game.IsActive;
            existingGame.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new GameDTO
            {
                Id = existingGame.Id,
                Name = existingGame.Name,
                Description = existingGame.Description,
                ImageUrl = existingGame.ImageUrl,
                GameUrl = existingGame.GameUrl,
                CategoryId = existingGame.CategoryId,
                IsActive = existingGame.IsActive,
                CreatedAt = existingGame.CreatedAt,
                UpdatedAt = existingGame.UpdatedAt ?? existingGame.CreatedAt
            };
        }

        public async Task DeleteGameAsync(int id)
        {
            var game = await _context.Games.FindAsync(id);
            if (game == null)
                throw new KeyNotFoundException($"Game with ID {id} not found.");

            _context.Games.Remove(game);
            await _context.SaveChangesAsync();
        }

        public async Task<List<GameDTO>> GetAllGamesAsync()
        {
            return await _context.Games
                .Include(g => g.Category)
                .Select(g => new GameDTO
                {
                    Id = g.Id,
                    Name = g.Name,
                    Description = g.Description,
                    ImageUrl = g.ImageUrl,
                    GameUrl = g.GameUrl,
                    CategoryId = g.CategoryId,
                    CategoryName = g.Category!.Name,
                    IsActive = g.IsActive,
                    CreatedAt = g.CreatedAt,
                    UpdatedAt = g.UpdatedAt ?? g.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<GameDTO> GetGameByIdAsync(int id)
        {
            var game = await _context.Games
                .Include(g => g.Category)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (game == null)
                return null!;

            return new GameDTO
            {
                Id = game.Id,
                Name = game.Name,
                Description = game.Description,
                ImageUrl = game.ImageUrl,
                GameUrl = game.GameUrl,
                CategoryId = game.CategoryId,
                CategoryName = game.Category!.Name,
                IsActive = game.IsActive,
                CreatedAt = game.CreatedAt,
                UpdatedAt = game.UpdatedAt ?? game.CreatedAt
            };
        }

        public async Task ToggleGameStatusAsync(int id)
        {
            var game = await _context.Games.FindAsync(id);
            if (game == null)
                throw new KeyNotFoundException($"Game with ID {id} not found.");

            game.IsActive = !game.IsActive;
            game.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }
} 