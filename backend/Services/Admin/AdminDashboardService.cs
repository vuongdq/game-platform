using GamePlatform.Data;
using GamePlatform.DTOs;
using GamePlatform.Models;
using Microsoft.EntityFrameworkCore;

namespace GamePlatform.Services.Admin
{
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly ApplicationDbContext _context;

        public AdminDashboardService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardStatsDTO> GetDashboardStatsAsync()
        {
            var totalGames = await _context.Games.CountAsync();
            var activeGames = await _context.Games.CountAsync(g => g.IsActive);
            var totalCategories = await _context.Categories.CountAsync();
            var totalUsers = await _context.Users.CountAsync();

            var recentGames = await _context.Games
                .Include(g => g.Category)
                .OrderByDescending(g => g.CreatedAt)
                .Take(5)
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

            return new DashboardStatsDTO
            {
                TotalGames = totalGames,
                ActiveGames = activeGames,
                TotalCategories = totalCategories,
                TotalUsers = totalUsers,
                RecentGames = recentGames
            };
        }

        public async Task<List<GameStatsDTO>> GetRecentGamesStatsAsync(int count = 5)
        {
            return await _context.Games
                .OrderByDescending(g => g.LastPlayed)
                .Take(count)
                .Select(g => new GameStatsDTO
                {
                    GameId = g.Id,
                    GameName = g.Name,
                    PlayCount = g.PlayCount,
                    LastPlayed = g.LastPlayed ?? g.CreatedAt
                })
                .ToListAsync();
        }
    }
} 