namespace ListMasterService.Models;

public class ListUpdateElementsRequest
{
    public Guid Id { get; set; }
    
    public Guid UserId { get; set; }
    public string Elements { get; set; }
}