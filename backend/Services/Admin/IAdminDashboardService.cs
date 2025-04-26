using GamePlatform.DTOs;

namespace GamePlatform.Services.Admin
{
    public interface IAdminDashboardService
    {
        Task<DashboardStatsDTO> GetDashboardStatsAsync();
        Task<List<GameStatsDTO>> GetRecentGamesStatsAsync(int count = 5);
    }
} 