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
    
    [HttpPost("duplicate_list")]
    public async Task<ActionResult> DuplicateList([FromBody] ListDuplicateRequest request)
    {
        if (request == null) return BadRequest("Пустой запрос");
        
        var jsonToken = new JwtSecurityTokenHandler().ReadToken(HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "")) as JwtSecurityToken;
        if (jsonToken == null) return BadRequest();
        var userId = jsonToken.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;
        if (userId != request.UserId.ToString()) return Unauthorized();
        
        var result = await _listsRepository.DuplicateList(request);
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
        if (string.IsNullOrEmpty(user_id) || string.IsNullOrEmpty(id)) return BadRequest("Пустой запрос");
        
        var jsonToken = new JwtSecurityTokenHandler().ReadToken(HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "")) as JwtSecurityToken;
        if (jsonToken == null) return BadRequest();
        var userId = jsonToken.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;
        if (userId != user_id) return Unauthorized();
        
        var result = await _listsRepository.DeleteList(new ListDeleteRequest()
        {
            Id = new Guid(id),
            UserId = new Guid(user_id)
        });
        
        return StatusCode(result.Code, result.Message);
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
        
        return StatusCode(result.Code, result.Message);
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
        
        return StatusCode(result.Code, result.Message);
    }
    
    [HttpDelete("delete_list_share")]
    public async Task<ActionResult> DeleteListShare([FromQuery] string user_id, [FromQuery] string list_id)
    {
        if (string.IsNullOrEmpty(user_id) || string.IsNullOrEmpty(list_id)) return BadRequest("Пустой запрос");
        
        var jsonToken = new JwtSecurityTokenHandler().ReadToken(HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "")) as JwtSecurityToken;
        if (jsonToken == null) return BadRequest();
        var userId = jsonToken.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;
        if (userId != user_id) return Unauthorized();
        
        var result = await _listsRepository.DeleteListShare(new DeleteListShareRequest()
        {
            Id = new Guid(list_id),
            UserId = new Guid(user_id)
        });
        
        return StatusCode(result.Code, result.Message);
    }
    
    [HttpDelete("cancel_share_for_user")]
    public async Task<ActionResult> CancelShareForUser([FromQuery] string owner_id, [FromQuery] string user_id, [FromQuery] string list_id)
    {
        if (string.IsNullOrEmpty(user_id) || string.IsNullOrEmpty(list_id) || string.IsNullOrEmpty(owner_id)) return BadRequest("Пустой запрос");
        
        var jsonToken = new JwtSecurityTokenHandler().ReadToken(HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "")) as JwtSecurityToken;
        if (jsonToken == null) return BadRequest();
        var userId = jsonToken.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;
        if (userId != owner_id) return Unauthorized();
        
        var result = await _listsRepository.CancelShareForUser(new DeleteListShareRequest()
        {
            Id = new Guid(list_id),
            UserId = new Guid(user_id)
        });
        
        return StatusCode(result.Code, result.Message);
    }

    [HttpGet("get_all_user_lists")]
    public async Task<ActionResult> GetAllUserLists([FromQuery] string user_id)
    {
        if (string.IsNullOrEmpty(user_id)) return BadRequest("Пустой запрос");
        
        var jsonToken = new JwtSecurityTokenHandler().ReadToken(HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "")) as JwtSecurityToken;
        if (jsonToken == null) return BadRequest();
        var userId = jsonToken.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;
        if (userId != user_id) return Unauthorized();
        
        var result = await _listsRepository.GetAllUserLists(new Guid(user_id));
        
        if (result == null)
        {
            return NotFound("Такого пользователя не существует");
        }
        return Ok(result);
    }
    
    [HttpGet("get_all_list_users")]
    public async Task<ActionResult> GetAllListUsers([FromQuery] string user_id, [FromQuery] string list_id)
    {
        if (string.IsNullOrEmpty(user_id) || string.IsNullOrEmpty(list_id)) return BadRequest("Пустой запрос");
        
        var jsonToken = new JwtSecurityTokenHandler().ReadToken(HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "")) as JwtSecurityToken;
        if (jsonToken == null) return BadRequest();
        var userId = jsonToken.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;
        if (userId != user_id) return Unauthorized();
        
        var result = await _listsRepository.GetAllListUsers(new Guid(list_id));
        
        if (result == null)
        {
            return NotFound("Такого списка не существует");
        }
        return Ok(result);
    }
    
    [HttpGet("get_list")]
    public async Task<ActionResult> GetList([FromQuery] string user_id, [FromQuery] string list_id)
    {
        if (string.IsNullOrEmpty(user_id) || string.IsNullOrEmpty(list_id)) return BadRequest("Ошибка запроса");
        
        var jsonToken = new JwtSecurityTokenHandler().ReadToken(HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "")) as JwtSecurityToken;
        if (jsonToken == null) return BadRequest();
        var userId = jsonToken.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;
        if (userId != user_id) return Unauthorized();
        
        var result = await _listsRepository.GetList(new GetListRequest()
        {
            Id = new Guid(list_id),
            UserId = new Guid(user_id)
        });
        
        if (result == null)
        {
            return NotFound("Такого списка не существует");
        }
        return Ok(result);
    }
    
}