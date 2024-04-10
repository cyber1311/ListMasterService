namespace ListMasterService.Models;

public class ListCreateRequest
{
    public Guid UserId { get; set; }
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Elements { get; set; }
    public bool IsShared { get; set; }
    public Guid OwnerId { get; set; }
}