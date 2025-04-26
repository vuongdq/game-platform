using GamePlatform.Data;
using GamePlatform.DTOs;
using GamePlatform.Models;
using Microsoft.EntityFrameworkCore;

namespace GamePlatform.Services.Public
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _context;

        public CategoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<CategoryDTO>> GetAllCategoriesAsync()
        {
            return await _context.Categories
                .Select(c => new CategoryDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    ImageUrl = c.ImageUrl,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt ?? c.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<CategoryDTO> GetCategoryByIdAsync(int id)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
                return null!;

            return new CategoryDTO
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                ImageUrl = category.ImageUrl,
                CreatedAt = category.CreatedAt,
                UpdatedAt = category.UpdatedAt ?? category.CreatedAt
            };
        }

        public async Task<List<GameDTO>> GetCategoryGamesAsync(int categoryId)
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
    }
} 