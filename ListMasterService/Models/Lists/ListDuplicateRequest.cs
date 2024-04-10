namespace ListMasterService.Models;

public class ListDuplicateRequest
{
    public Guid ListId { get; set; }
    public Guid NewListId { get; set; }
    public Guid UserId { get; set; }
}