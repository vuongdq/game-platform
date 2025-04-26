using GamePlatform.Data;
using GamePlatform.DTOs;
using GamePlatform.Models;
using Microsoft.EntityFrameworkCore;

namespace GamePlatform.Services.Admin
{
    public class AdminCategoryService : IAdminCategoryService
    {
        private readonly ApplicationDbContext _context;

        public AdminCategoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CategoryDTO> CreateCategoryAsync(CategoryDTO category)
        {
            var newCategory = new Category
            {
                Name = category.Name,
                Description = category.Description,
                ImageUrl = category.ImageUrl
            };

            _context.Categories.Add(newCategory);
            await _context.SaveChangesAsync();

            return new CategoryDTO
            {
                Id = newCategory.Id,
                Name = newCategory.Name,
                Description = newCategory.Description,
                ImageUrl = newCategory.ImageUrl
            };
        }

        public async Task<CategoryDTO> UpdateCategoryAsync(int id, CategoryDTO category)
        {
            var existingCategory = await _context.Categories.FindAsync(id);
            if (existingCategory == null)
                throw new KeyNotFoundException($"Category with ID {id} not found.");

            existingCategory.Name = category.Name;
            existingCategory.Description = category.Description;
            existingCategory.ImageUrl = category.ImageUrl;

            await _context.SaveChangesAsync();

            return new CategoryDTO
            {
                Id = existingCategory.Id,
                Name = existingCategory.Name,
                Description = existingCategory.Description,
                ImageUrl = existingCategory.ImageUrl
            };
        }

        public async Task DeleteCategoryAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                throw new KeyNotFoundException($"Category with ID {id} not found.");

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }

        public async Task<List<CategoryDTO>> GetAllCategoriesAsync()
        {
            return await _context.Categories
                .Select(c => new CategoryDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    ImageUrl = c.ImageUrl
                })
                .ToListAsync();
        }

        public async Task<CategoryDTO> GetCategoryByIdAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return null;

            return new CategoryDTO
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                ImageUrl = category.ImageUrl
            };
        }
    }
} 