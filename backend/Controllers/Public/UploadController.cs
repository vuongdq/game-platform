using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Web;
using Microsoft.AspNetCore.Http.Features;

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

    [HttpPost("image")]
    [Authorize(Roles = "Admin")]
    [RequestSizeLimit(5 * 1024 * 1024)] // 5MB
    public async Task<IActionResult> UploadImage(IFormFile file)
    {
        try
        {
            _logger.LogInformation("Starting image upload process");

            if (file == null || file.Length == 0)
            {
                _logger.LogWarning("No file was uploaded");
                return BadRequest(new { message = "Vui lòng chọn file ảnh để upload" });
            }

            _logger.LogInformation($"Received file: {file.FileName}, Size: {file.Length} bytes, ContentType: {file.ContentType}");

            // Validate file type
            var allowedImageTypes = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
            var allowedContentTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/bmp", "image/webp" };
            
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var contentType = file.ContentType.ToLowerInvariant();

            if (!allowedImageTypes.Contains(fileExtension) || !allowedContentTypes.Contains(contentType))
            {
                _logger.LogWarning($"Invalid file type: {fileExtension}, ContentType: {contentType}");
                return BadRequest(new { message = $"File không hợp lệ. Vui lòng upload file ảnh (jpg, jpeg, png, gif, bmp, webp). File hiện tại: {fileExtension}" });
            }

            // Validate file size (max 5MB)
            const int maxFileSize = 5 * 1024 * 1024; // 5MB
            if (file.Length > maxFileSize)
            {
                _logger.LogWarning($"File too large: {file.Length} bytes");
                return BadRequest(new { message = "File quá lớn. Kích thước tối đa là 5MB" });
            }

            // Get the absolute path to the wwwroot directory
            var webRootPath = _environment.WebRootPath;
            if (string.IsNullOrEmpty(webRootPath))
            {
                webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            }

            // Create uploads directory if it doesn't exist
            var uploadsPath = Path.Combine(webRootPath, "uploads");
            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }

            // Create image directory if it doesn't exist
            var imagePath = Path.Combine(uploadsPath, "image");
            if (!Directory.Exists(imagePath))
            {
                Directory.CreateDirectory(imagePath);
            }

            _logger.LogInformation($"Upload path: {imagePath}");

            // Generate a unique filename
            var fileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(imagePath, fileName);
            _logger.LogInformation($"Saving file to: {filePath}");

            try
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Return the relative path
                var relativePath = $"/uploads/image/{fileName}";
                _logger.LogInformation($"File uploaded successfully: {relativePath}");
                return Ok(new { path = relativePath });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving file");
                return StatusCode(500, new { message = "Lỗi khi lưu file" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading image");
            return StatusCode(500, new { message = $"Lỗi khi upload ảnh: {ex.Message}" });
        }
    }

    [HttpPost("game")]
    [Authorize(Roles = "Admin")]
    [RequestSizeLimit(50 * 1024 * 1024)] // 50MB
    public async Task<IActionResult> UploadGame(IFormFile file)
    {
        try
        {
            _logger.LogInformation("Starting game upload process");

            if (file == null || file.Length == 0)
            {
                _logger.LogWarning("No file was uploaded");
                return BadRequest(new { message = "Vui lòng chọn file game để upload" });
            }

            _logger.LogInformation($"Received file: {file.FileName}, Size: {file.Length} bytes, ContentType: {file.ContentType}");

            // Validate file type
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var contentType = file.ContentType.ToLowerInvariant();

            if (fileExtension != ".swf" || !contentType.Contains("application/x-shockwave-flash"))
            {
                _logger.LogWarning($"Invalid file type: {fileExtension}, ContentType: {contentType}");
                return BadRequest(new { message = $"File không hợp lệ. Vui lòng upload file game định dạng .swf. File hiện tại: {fileExtension}" });
            }

            // Validate file size (max 50MB)
            const int maxFileSize = 50 * 1024 * 1024; // 50MB
            if (file.Length > maxFileSize)
            {
                _logger.LogWarning($"File too large: {file.Length} bytes");
                return BadRequest(new { message = "File quá lớn. Kích thước tối đa là 50MB" });
            }

            // Get the absolute path to the wwwroot directory
            var webRootPath = _environment.WebRootPath;
            if (string.IsNullOrEmpty(webRootPath))
            {
                webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            }

            // Create uploads directory if it doesn't exist
            var uploadsPath = Path.Combine(webRootPath, "uploads");
            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }

            // Create game directory if it doesn't exist
            var gamePath = Path.Combine(uploadsPath, "game");
            if (!Directory.Exists(gamePath))
            {
                Directory.CreateDirectory(gamePath);
            }

            _logger.LogInformation($"Upload path: {gamePath}");

            // Generate a unique filename
            var fileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(gamePath, fileName);
            _logger.LogInformation($"Saving file to: {filePath}");

            try
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Return the relative path
                var relativePath = $"/uploads/game/{fileName}";
                _logger.LogInformation($"File uploaded successfully: {relativePath}");
                return Ok(new { path = relativePath });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving file");
                return StatusCode(500, new { message = "Lỗi khi lưu file" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading game");
            return StatusCode(500, new { message = $"Lỗi khi upload game: {ex.Message}" });
        }
    }
} 