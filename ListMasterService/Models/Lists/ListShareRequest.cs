namespace ListMasterService.Models;

public class ListShareRequest
{
    public Guid ListId { get; set; }
    public Guid UserOwnerId { get; set; }
    public string NewUserEmail { get; set; }
}