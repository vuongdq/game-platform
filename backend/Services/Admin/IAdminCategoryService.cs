using GamePlatform.DTOs;

namespace GamePlatform.Services.Admin
{
    public interface IAdminCategoryService
    {
        Task<CategoryDTO> CreateCategoryAsync(CategoryDTO category);
        Task<CategoryDTO> UpdateCategoryAsync(int id, CategoryDTO category);
        Task DeleteCategoryAsync(int id);
        Task<List<CategoryDTO>> GetAllCategoriesAsync();
        Task<CategoryDTO> GetCategoryByIdAsync(int id);
    }
} 