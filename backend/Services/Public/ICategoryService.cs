using GamePlatform.DTOs;

namespace GamePlatform.Services.Public
{
    public interface ICategoryService
    {
        Task<List<CategoryDTO>> GetAllCategoriesAsync();
        Task<CategoryDTO> GetCategoryByIdAsync(int id);
        Task<List<GameDTO>> GetCategoryGamesAsync(int categoryId);
    }
} 