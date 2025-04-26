using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GamePlatform.DTOs;
using GamePlatform.Services.Admin;

namespace GamePlatform.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Route("api/admin/dashboard")]
    [ApiController]
    public class AdminDashboardController : ControllerBase
    {
        private readonly IAdminDashboardService _dashboardService;

        public AdminDashboardController(IAdminDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("stats")]
        public async Task<ActionResult<DashboardStatsDTO>> GetDashboardStats()
        {
            var stats = await _dashboardService.GetDashboardStatsAsync();
            return Ok(stats);
        }

        [HttpGet("recent-games")]
        public async Task<ActionResult<List<GameStatsDTO>>> GetRecentGamesStats([FromQuery] int count = 5)
        {
            var recentGames = await _dashboardService.GetRecentGamesStatsAsync(count);
            return Ok(recentGames);
        }
    }
} 