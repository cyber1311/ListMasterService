using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ListMasterService.Database;
using ListMasterService.Models;
using ListMasterService.Models.Groups;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace ListMasterService.Controllers;

[Authorize]
[Route("groups")]
public class GroupController : Controller
{
    private readonly IGroupsRepository _groupsRepository;


    public GroupController(IGroupsRepository groupsRepository)
    {
        _groupsRepository = groupsRepository;
    }
    
    [HttpPost("add_group")]
    public async Task<ActionResult> AddGroup([FromBody] Group request)
    {
        if (request == null) return BadRequest( "Пустой запрос");

        var jsonToken = new JwtSecurityTokenHandler().ReadToken(HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "")) as JwtSecurityToken;
        if (jsonToken == null) return BadRequest();
        var userId = jsonToken.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;
        if (userId != request.OwnerId.ToString()) return Unauthorized();
        var result = await _groupsRepository.AddGroup(request);
        
        return StatusCode(result.Code, result.Message);
    }
    
    [HttpDelete("delete_group")]
    public async Task<ActionResult> DeleteGroup([FromQuery] string user_id, [FromQuery] string group_id)
    {
        if (user_id is null or "" || group_id is "" or null) return BadRequest("Пустой запрос");
        
        var jsonToken = new JwtSecurityTokenHandler().ReadToken(HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "")) as JwtSecurityToken;
        if (jsonToken == null) return BadRequest();
        var userId = jsonToken.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;
        if (userId != user_id.ToString()) return Unauthorized();
        
        var result = await _groupsRepository.DeleteGroup(new GroupDeleteRequest()
        {
            UserId = new Guid(user_id),
            GroupId = new Guid(group_id)
        });
        return StatusCode(result.Code, result.Message);
    }
    
    [HttpPut("update_group_title")]
    public async Task<ActionResult> UpdateGroupTitle([FromBody] GroupUpdateTitleRequest request)
    {
        if (request == null) return BadRequest("Пустой запрос");
        
        var jsonToken = new JwtSecurityTokenHandler().ReadToken(HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "")) as JwtSecurityToken;
        if (jsonToken == null) return Unauthorized();
        var userId = jsonToken.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;
        if (userId != request.UserId.ToString()) return Unauthorized();
        
        var result = await _groupsRepository.UpdateGroupTitle(request);
        return StatusCode(result.Code, result.Message);
    }
    
    [HttpPost("add_group_member")]
    public async Task<ActionResult> AddGroupMember([FromBody] GroupMemberAddRequest request)
    {
        if (request == null) return BadRequest( "Пустой запрос");

        var jsonToken = new JwtSecurityTokenHandler().ReadToken(HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "")) as JwtSecurityToken;
        if (jsonToken == null) return BadRequest();
        var userId = jsonToken.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;
        if (userId != request.UserId.ToString()) return Unauthorized();
        
        var result = await _groupsRepository.AddGroupMember(request);
        return StatusCode(result.Code, result.Message);
    }
    
    [HttpDelete("delete_group_member")]
    public async Task<ActionResult> DeleteGroupMember([FromQuery] string user_id, [FromQuery] string group_id, [FromQuery] string user_to_delete_id)
    {
        if (user_id is null or "" || group_id is "" or null || user_to_delete_id is "" or null) return BadRequest("Пустой запрос");
        
        var jsonToken = new JwtSecurityTokenHandler().ReadToken(HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "")) as JwtSecurityToken;
        if (jsonToken == null) return BadRequest();
        var userId = jsonToken.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;
        if (userId != user_id) return Unauthorized();
        
        var result = await _groupsRepository.DeleteGroupMember(new GroupMemberDeleteRequest()
        {
            UserId = new Guid(user_id),
            GroupId = new Guid(group_id),
            UserToDeleteId = new Guid(user_to_delete_id)
        });
        return StatusCode(result.Code, result.Message);
    }
    
    [HttpGet("get_all_user_groups")]
    public async Task<ActionResult> GetAllUserGroups([FromQuery] string user_id)
    {
        if (user_id == null) return BadRequest("Пустой запрос");
        
        var jsonToken = new JwtSecurityTokenHandler().ReadToken(HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "")) as JwtSecurityToken;
        if (jsonToken == null) return BadRequest();
        var userId = jsonToken.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;
        if (userId != user_id) return Unauthorized();
        
        var result = await _groupsRepository.GetAllUserGroups(new Guid(user_id));
        if (result == null)
        {
            return NotFound("Такого пользователя не существует");
        }
        return Ok(result);
    }
    
    [HttpGet("get_all_group_members")]
    public async Task<ActionResult> GetAllGroupMembers([FromQuery] string user_id, [FromQuery] string group_id)
    {
        if (user_id == null) return BadRequest("Пустой запрос");
        
        var jsonToken = new JwtSecurityTokenHandler().ReadToken(HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "")) as JwtSecurityToken;
        if (jsonToken == null) return BadRequest();
        var userId = jsonToken.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;
        if (userId != user_id) return Unauthorized();
        
        var result = await _groupsRepository.GetAllGroupMembers(new Guid(group_id));
        if (result == null)
        {
            return NotFound("Такой группы не существует");
        }
        return Ok(result);
    }
    
}