namespace ListMasterService.Models.Groups;

public class GroupDeleteRequest
{
    public Guid GroupId { get; set; }
    public Guid UserId { get; set; }
}