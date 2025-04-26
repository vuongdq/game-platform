using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GamePlatform.Data;
using GamePlatform.Models;

namespace GamePlatform.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<CategoryController> _logger;

    public CategoryController(ApplicationDbContext context, ILogger<CategoryController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetCategories()
    {
        try
        {
            var categories = await _context.Categories
                .Select(c => new
                {
                    c.Id,
                    c.Name,
                    c.Description,
                    c.ImageUrl,
                    c.CreatedAt,
                    c.UpdatedAt
                })
                .ToListAsync();

            return Ok(categories);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting categories");
            return StatusCode(500, new { message = "An error occurred while retrieving categories" });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<object>> GetCategory(int id)
    {
        try
        {
            var category = await _context.Categories
                .Select(c => new
                {
                    c.Id,
                    c.Name,
                    c.Description,
                    c.ImageUrl,
                    c.CreatedAt,
                    c.UpdatedAt
                })
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return NotFound(new { message = $"Category with ID {id} not found" });
            }

            return Ok(category);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting category with ID {Id}", id);
            return StatusCode(500, new { message = "An error occurred while retrieving the category" });
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<object>> CreateCategory([FromBody] Category category)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid category data", errors = ModelState.Values.SelectMany(v => v.Errors) });
            }

            var existingCategory = await _context.Categories
                .FirstOrDefaultAsync(c => c.Name.ToLower() == category.Name.ToLower());
            
            if (existingCategory != null)
            {
                return BadRequest(new { message = "A category with this name already exists" });
            }

            category.CreatedAt = DateTime.UtcNow;
            category.UpdatedAt = DateTime.UtcNow;
            
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            var createdCategory = new
            {
                category.Id,
                category.Name,
                category.Description,
                category.ImageUrl,
                category.CreatedAt,
                category.UpdatedAt
            };

            return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, createdCategory);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating category");
            return StatusCode(500, new { message = "An error occurred while creating the category" });
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryUpdateRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid category data", errors = ModelState.Values.SelectMany(v => v.Errors) });
            }

            var existingCategory = await _context.Categories.FindAsync(id);
            if (existingCategory == null)
            {
                return NotFound(new { message = $"Category with ID {id} not found" });
            }

            var duplicateCategory = await _context.Categories
                .FirstOrDefaultAsync(c => c.Name.ToLower() == request.Name.ToLower() && c.Id != id);
            
            if (duplicateCategory != null)
            {
                return BadRequest(new { message = "A category with this name already exists" });
            }

            existingCategory.Name = request.Name;
            existingCategory.Description = request.Description;
            existingCategory.ImageUrl = request.ImageUrl;
            existingCategory.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { 
                    message = "Category updated successfully",
                    category = new {
                        existingCategory.Id,
                        existingCategory.Name,
                        existingCategory.Description,
                        existingCategory.ImageUrl,
                        existingCategory.CreatedAt,
                        existingCategory.UpdatedAt
                    }
                });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(id))
                {
                    return NotFound(new { message = $"Category with ID {id} not found" });
                }
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating category with ID {Id}", id);
            return StatusCode(500, new { message = "An error occurred while updating the category" });
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        try
        {
            var category = await _context.Categories
                .Include(c => c.Games)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return NotFound(new { message = $"Category with ID {id} not found" });
            }

            if (category.Games?.Any() == true)
            {
                var gameTitles = category.Games.Select(g => g.Title).ToList();
                return BadRequest(new { 
                    message = "Cannot delete category with associated games",
                    games = gameTitles,
                    count = gameTitles.Count
                });
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Category deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting category with ID {Id}", id);
            return StatusCode(500, new { message = "An error occurred while deleting the category" });
        }
    }

    private bool CategoryExists(int id)
    {
        return _context.Categories.Any(e => e.Id == id);
    }
}

public class CategoryUpdateRequest
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string ImageUrl { get; set; }
} 