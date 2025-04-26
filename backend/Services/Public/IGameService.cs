using GamePlatform.DTOs;

namespace GamePlatform.Services.Public
{
    public interface IGameService
    {
        Task<List<GameDTO>> GetAllGamesAsync();
        Task<GameDTO> GetGameByIdAsync(int id);
        Task<List<GameDTO>> GetGamesByCategoryAsync(int categoryId);
        Task<List<GameDTO>> GetPopularGamesAsync(int count);
        Task<List<GameDTO>> GetRecentlyPlayedGamesAsync(int count);
        Task UpdateGameStatsAsync(int gameId);
    }
} 