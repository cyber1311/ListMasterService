namespace ListMasterService.Models;

public class ListCopyRequest
{
    public Guid ListId { get; set; }
    public Guid NewListId { get; set; }
    public Guid UserOwnerId { get; set; }
    public Guid NewUserId { get; set; }
}