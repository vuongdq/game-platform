using System.ComponentModel.DataAnnotations;

namespace GamePlatform.DTOs
{
    public class GameDTO
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
        
        [Required]
        [StringLength(500)]
        public string ImageUrl { get; set; } = string.Empty;
        
        [Required]
        [StringLength(500)]
        public string GameUrl { get; set; } = string.Empty;
        
        public int CategoryId { get; set; }
        
        [Required]
        public string CategoryName { get; set; } = string.Empty;
        
        public bool IsActive { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime UpdatedAt { get; set; }
    }
} 