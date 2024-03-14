using System.IdentityModel.Tokens.Jwt;
using ListMasterService.Models.Images;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ListMasterService.Controllers;

[Authorize]
[Route("images")]
public class ImageController : Controller
{
    public ImageController()
    {
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile image, string userId)
    {
        try
        {
            if (userId == null) return BadRequest("Пустой запрос");
            
            if (image == null || image.Length == 0)
            {
                return BadRequest("Invalid image file");
            }
            
            var jsonToken = new JwtSecurityTokenHandler().ReadToken(HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "")) as JwtSecurityToken;
            if (jsonToken == null) return BadRequest();
            var user_id = jsonToken.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;
            if (user_id != userId.ToString()) return Unauthorized();

            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", user_id);
            
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
    public async Task<IActionResult> Download([FromBody] UploadRequest request)
    {
        try
        {
            if (request == null) return BadRequest("Пустой запрос");

            var jsonToken = new JwtSecurityTokenHandler().ReadToken(HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "")) as JwtSecurityToken;
            if (jsonToken == null) return BadRequest();
            var userId = jsonToken.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;
            if (userId != request.UserId.ToString()) return Unauthorized();
            
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", userId, request.ImageName);

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
}