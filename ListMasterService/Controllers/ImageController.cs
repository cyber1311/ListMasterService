using Microsoft.AspNetCore.Mvc;

namespace ListMasterService.Controllers;

[Route("images")]
public class ImageController : Controller
{
    public ImageController()
    {
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile image)
    {
        try
        {
            
            if (image == null || image.Length == 0)
            {
                return BadRequest("Invalid image file");
            }
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            
            var filePath = Path.Combine(path, image.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            return Ok("Файл загружен");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }
    
    [HttpGet("download")]
    public async Task<IActionResult> Download([FromQuery] string image_name)
    {
        try
        {
            if (image_name is null or "") return BadRequest("Пустой запрос");
            
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", image_name);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            var imageBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            
            return File(imageBytes, "image/jpeg");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }
    
    [HttpDelete("delete")]
    public IActionResult Delete([FromQuery] string image_name)
    {
        try
        {
            if (string.IsNullOrEmpty(image_name))
                return BadRequest("Пустой запрос");

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", image_name);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            System.IO.File.Delete(filePath);

            return Ok("Файл удален");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }
}