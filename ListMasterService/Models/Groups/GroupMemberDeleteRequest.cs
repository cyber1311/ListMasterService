namespace ListMasterService.Models.Groups;

public class GroupMemberDeleteRequest
{
    public Guid GroupId { get; set; }
    public Guid UserId { get; set; }
    public Guid UserToDeleteId { get; set; }
}