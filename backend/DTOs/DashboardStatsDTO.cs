namespace GamePlatform.DTOs
{
    public class DashboardStatsDTO
    {
        public int TotalGames { get; set; }
        public int ActiveGames { get; set; }
        public int TotalCategories { get; set; }
        public int TotalUsers { get; set; }
        public List<GameDTO> RecentGames { get; set; } = new();
    }

    public class GameStatsDTO
    {
        public int GameId { get; set; }
        public string? GameName { get; set; }
        public int PlayCount { get; set; }
        public DateTime? LastPlayed { get; set; }
    }
} 