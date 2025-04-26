using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Web;

namespace GamePlatform.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UploadController : ControllerBase
{
    private readonly ILogger<UploadController> _logger;
    private readonly IWebHostEnvironment _environment;

    public UploadController(ILogger<UploadController> logger, IWebHostEnvironment environment)
    {
        _logger = logger;
        _environment = environment;
    }

    [HttpPost]
    public async Task<IActionResult> UploadFile([FromForm] IFormFile file, [FromForm] string type)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { message = "No file was uploaded" });
            }

            // Validate file type
            var allowedImageTypes = new[] { "image/jpeg", "image/png", "image/gif" };
            var allowedGameTypes = new[] { "application/zip", "application/x-rar-compressed", "application/x-7z-compressed" };

            if (type == "image" && !allowedImageTypes.Contains(file.ContentType))
            {
                return BadRequest(new { message = "Invalid image file type. Only JPG, PNG and GIF are allowed" });
            }

            if (type == "game" && !allowedGameTypes.Contains(file.ContentType))
            {
                return BadRequest(new { message = "Invalid game file type. Only ZIP, RAR and 7Z are allowed" });
            }

            // Create upload directory if it doesn't exist
            var uploadPath = Path.Combine(_environment.WebRootPath, "uploads", type);
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            // Generate unique filename
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadPath, fileName);

            // Save file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Return file URL
            var fileUrl = $"/uploads/{type}/{fileName}";
            return Ok(new { url = fileUrl });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file");
            return StatusCode(500, new { message = "An error occurred while uploading the file" });
        }
    }
} 