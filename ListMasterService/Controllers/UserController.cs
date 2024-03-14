using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ListMasterService.Database;
using ListMasterService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace ListMasterService.Controllers;


[Route("users")]
public class UserController : Controller
{
    private readonly IUsersRepository _usersRepository;


    public UserController(IUsersRepository usersRepository)
    {
        _usersRepository = usersRepository;
    }
    
    [HttpPost("add_user")]
    public async Task<ActionResult> AddUser([FromBody] User user)
    {
        if (user == null) return BadRequest( "Пустой запрос");

        var existsUser = await _usersRepository.ExistsUser(user.Email);
        
        if(existsUser) return StatusCode(409, "Такой пользователь уже существует");
        
        var result = await _usersRepository.AddUser(user);
        if (result.Code != 200) return StatusCode(result.Code, result.Message);
    
        var claims = new List<Claim> {new Claim(ClaimTypes.Name, user.Id.ToString()) };
       
        var jwt = new JwtSecurityToken(
            issuer: AuthOptions.ISSUER,
            audience: AuthOptions.AUDIENCE,
            claims: claims,
            expires: DateTime.UtcNow.Add(TimeSpan.FromDays(30)),
            signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(),
                SecurityAlgorithms.HmacSha256));
            
        return Ok(new RegistrationResponse()
        {
            Token = new JwtSecurityTokenHandler().WriteToken(jwt),
            ExpiresAt = jwt.ValidTo.ToString()
        });
    }
    
    [HttpGet("sign_in")]
    public async Task<ActionResult> SignIn([FromQuery] string email, [FromQuery] string password)
    {
        var existsUser = await _usersRepository.GetUser(new GetUserByEmailRequest()
        {
            Email = email
        });
        if(existsUser.Email == null) return NotFound("Такого пользователя не существует");
        if(existsUser.Password != password) NotFound("Неверный пароль");
        var claims = new List<Claim> {new Claim(ClaimTypes.Name, existsUser.Id.ToString()) };
       
        var jwt = new JwtSecurityToken(
            issuer: AuthOptions.ISSUER,
            audience: AuthOptions.AUDIENCE,
            claims: claims,
            expires: DateTime.UtcNow.Add(TimeSpan.FromDays(30)),
            signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            
        return Ok(new SignInResponse()
        {
            Id = existsUser.Id,
            Name = existsUser.Name,
            Token = new JwtSecurityTokenHandler().WriteToken(jwt),
            ExpiresAt = jwt.ValidTo.ToString()
        });
    }
    
    [Authorize]
    [HttpDelete("delete_user")]
    public async Task<ActionResult> DeleteUser([FromBody] UserDeleteRequest request)
    {
        if (request == null) return BadRequest("Пустой запрос");
        
        var jsonToken = new JwtSecurityTokenHandler().ReadToken(HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "")) as JwtSecurityToken;
        if (jsonToken == null) return BadRequest();
        var userId = jsonToken.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;
        if (userId != request.Id.ToString()) return Unauthorized();
        
        var result = await _usersRepository.DeleteUser(request);
        return StatusCode(result.Code, result.Message);
    }
    
    [Authorize]
    [HttpPut("update_user_name")]
    public async Task<ActionResult> UpdateUserName([FromBody] UserUpdateNameRequest request)
    {
        if (request == null) return BadRequest("Пустой запрос");
        
        var jsonToken = new JwtSecurityTokenHandler().ReadToken(HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "")) as JwtSecurityToken;
        if (jsonToken == null) return Unauthorized();
        var userId = jsonToken.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;
        if (userId != request.Id.ToString()) return Unauthorized();
        
        var result = await _usersRepository.UpdateUserName(request);
        return StatusCode(result.Code, result.Message);
    }
    
    [Authorize]
    [HttpPut("update_user_email")]
    public async Task<ActionResult> UpdateUserEmail([FromBody] UserUpdateEmailRequest request)
    {
        if (request == null) return BadRequest("Пустой запрос");
        
        var jsonToken = new JwtSecurityTokenHandler().ReadToken(HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "")) as JwtSecurityToken;
        if (jsonToken == null) return BadRequest();
        var userId = jsonToken.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;
        if (userId != request.Id.ToString()) return Unauthorized();
        
        var result = await _usersRepository.UpdateUserEmail(request);
        return StatusCode(result.Code, result.Message);
    }
    
    [Authorize]
    [HttpPut("update_user_password")]
    public async Task<ActionResult> UpdateUserPassword([FromBody] UserUpdatePasswordRequest request)
    {
        if (request == null)  return BadRequest("Пустой запрос");
        
        var jsonToken = new JwtSecurityTokenHandler().ReadToken(HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "")) as JwtSecurityToken;
        if (jsonToken == null) return BadRequest();
        var userId = jsonToken.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;
        if (userId != request.Id.ToString()) return Unauthorized();
        
        var result = await _usersRepository.UpdateUserPassword(request);
        return StatusCode(result.Code, result.Message);
    }
}