namespace ListMasterService.Models;

public class ListUpdateTitleRequest
{
    public Guid Id { get; set; }
    
    public Guid UserId { get; set; }
    public string Title { get; set; }
}