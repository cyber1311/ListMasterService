namespace ListMasterService.Models.Groups;

public class GroupMemberAddRequest
{
    public Guid GroupId { get; set; }
    public Guid UserId { get; set; }
    public string UserToAddEmail { get; set; }
}