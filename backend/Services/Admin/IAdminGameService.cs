using GamePlatform.DTOs;

namespace GamePlatform.Services.Admin
{
    public interface IAdminGameService
    {
        Task<GameDTO> CreateGameAsync(GameDTO game);
        Task<GameDTO> UpdateGameAsync(int id, GameDTO game);
        Task DeleteGameAsync(int id);
        Task<List<GameDTO>> GetAllGamesAsync();
        Task<GameDTO> GetGameByIdAsync(int id);
        Task ToggleGameStatusAsync(int id);
    }
} 