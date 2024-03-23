using System.IdentityModel.Tokens.Jwt;
using ListMasterService.Database;
using ListMasterService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ListMasterService.Controllers;

[Authorize]
[Route("lists")]
public class ListController : Controller
{
    
    private readonly IListsRepository _listsRepository;


    public ListController(IListsRepository listsRepository)
    {
        _listsRepository = listsRepository;
    }
    
    [HttpPost("add_list")]
    public async Task<ActionResult> AddList([FromBody] ListCreateRequest request)
    {
        if (request == null) return BadRequest("Пустой запрос");
        
        var jsonToken = new JwtSecurityTokenHandler().ReadToken(HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "")) as JwtSecurityToken;
        if (jsonToken == null) return BadRequest();
        var userId = jsonToken.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;
        if (userId != request.UserId.ToString()) return Unauthorized();
        
        var result = await _listsRepository.AddList(request);
        return StatusCode(result.Code, result.Message);
    }
    
    [HttpPost("copy_list")]
    public async Task<ActionResult> CopyList([FromBody] ListCopyRequest request)
    {
        if (request == null) return BadRequest("Пустой запрос");
        
        var jsonToken = new JwtSecurityTokenHandler().ReadToken(HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "")) as JwtSecurityToken;
        if (jsonToken == null) return BadRequest();
        var userId = jsonToken.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;
        if (userId != request.UserOwnerId.ToString()) return Unauthorized();
        
        var result = await _listsRepository.CopyList(request);
        return StatusCode(result.Code, result.Message);
    }
    
    [HttpPost("share_list")]
    public async Task<ActionResult> ShareList([FromBody] ListShareRequest request)
    {
        if (request == null) return BadRequest("Пустой запрос");
        
        var jsonToken = new JwtSecurityTokenHandler().ReadToken(HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "")) as JwtSecurityToken;
        if (jsonToken == null) return BadRequest();
        var userId = jsonToken.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;
        if (userId != request.UserOwnerId.ToString()) return Unauthorized();
        
        var result = await _listsRepository.ShareList(request);
        return StatusCode(result.Code, result.Message);
    }
    
    [HttpDelete("delete_list")]
    public async Task<ActionResult> DeleteList([FromQuery] string user_id, [FromQuery] string id)
    {
        Console.WriteLine(user_id);
        Console.WriteLine(id);
        if (user_id == null || id == null) return BadRequest("Пустой запрос");
        
        var jsonToken = new JwtSecurityTokenHandler().ReadToken(HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "")) as JwtSecurityToken;
        if (jsonToken == null) return BadRequest();
        var userId = jsonToken.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;
        if (userId != user_id) return Unauthorized();
        
        var result = await _listsRepository.DeleteList(new ListDeleteRequest()
        {
            Id = new Guid(id),
            UserId = new Guid(user_id)
        });
        if (result.Code == 500)
        {
            return BadRequest("Такого списка не существует");
        }
        return Ok("Список удален");
    }

    [HttpPut("update_list_title")]
    public async Task<ActionResult> UpdateListTitle([FromBody] ListUpdateTitleRequest request)
    {
        if (request == null) return BadRequest("Пустой запрос");
        
        var jsonToken = new JwtSecurityTokenHandler().ReadToken(HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "")) as JwtSecurityToken;
        if (jsonToken == null) return BadRequest();
        var userId = jsonToken.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;
        if (userId != request.UserId.ToString()) return Unauthorized();
        
        var result = await _listsRepository.UpdateListTitle(request);
        if (result.Code == 500)
        {
            return BadRequest("Такого списка не существует");
        }
        return Ok("Название списка обновлено");
    }

    [HttpPut("update_list_elements")]
    public async Task<ActionResult> UpdateListElements([FromBody] ListUpdateElementsRequest request)
    {
        if (request == null) return BadRequest("Пустой запрос");
        
        var jsonToken = new JwtSecurityTokenHandler().ReadToken(HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "")) as JwtSecurityToken;
        if (jsonToken == null) return BadRequest();
        var userId = jsonToken.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;
        if (userId != request.UserId.ToString()) return Unauthorized();
        
        var result = await _listsRepository.UpdateListElements(request);
        if (result.Code == 500)
        {
            return BadRequest("Такого списка не существует");
        }
        return Ok("Список обновлен");
    }
    
    [HttpGet("get_list")]
    public async Task<ActionResult> GetList([FromBody] GetListRequest request)
    {
        if (request == null) return BadRequest("Пустой запрос");
        
        var jsonToken = new JwtSecurityTokenHandler().ReadToken(HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "")) as JwtSecurityToken;
        if (jsonToken == null) return BadRequest();
        var userId = jsonToken.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;
        if (userId != request.UserId.ToString()) return Unauthorized();
        
        var result = await _listsRepository.GetList(request);
        if (result == null)
        {
            return BadRequest("Такого списка не существует");
        }
        return Ok(result);
    }
    
    [HttpGet("get_all_user_lists")]
    public async Task<ActionResult> GetAllUserLists([FromQuery] string user_id)
    {
        if (user_id == null) return BadRequest("Пустой запрос");
        
        var jsonToken = new JwtSecurityTokenHandler().ReadToken(HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "")) as JwtSecurityToken;
        if (jsonToken == null) return BadRequest();
        var userId = jsonToken.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;
        if (userId != user_id) return Unauthorized();
        
        var result = await _listsRepository.GetAllUserLists(new GetAllUserListsRequest()
        {
            UserId = new Guid(user_id)
        });
        if (result == null)
        {
            return BadRequest("Такого пользователя не существует");
        }
        return Ok(result);
    }
    
}