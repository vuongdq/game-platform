using GamePlatform.Data;
using GamePlatform.DTOs;
using GamePlatform.Models;
using Microsoft.EntityFrameworkCore;

namespace GamePlatform.Services.Public
{
    public class GameService : IGameService
    {
        private readonly ApplicationDbContext _context;

        public GameService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<GameDTO>> GetAllGamesAsync()
        {
            return await _context.Games
                .Include(g => g.Category)
                .Where(g => g.IsActive)
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
                .FirstOrDefaultAsync(g => g.Id == id && g.IsActive);

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

        public async Task<List<GameDTO>> GetGamesByCategoryAsync(int categoryId)
        {
            return await _context.Games
                .Include(g => g.Category)
                .Where(g => g.CategoryId == categoryId && g.IsActive)
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

        public async Task<List<GameDTO>> GetPopularGamesAsync(int count)
        {
            return await _context.Games
                .Include(g => g.Category)
                .Where(g => g.IsActive)
                .OrderByDescending(g => g.PlayCount)
                .Take(count)
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

        public async Task<List<GameDTO>> GetRecentlyPlayedGamesAsync(int count)
        {
            return await _context.Games
                .Include(g => g.Category)
                .Where(g => g.IsActive && g.LastPlayed != null)
                .OrderByDescending(g => g.LastPlayed)
                .Take(count)
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

        public async Task UpdateGameStatsAsync(int gameId)
        {
            var game = await _context.Games.FindAsync(gameId);
            if (game != null)
            {
                game.PlayCount++;
                game.LastPlayed = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }
    }
} 