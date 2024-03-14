namespace ListMasterService.Models;

public class GetListRequest
{
    public Guid UserId { get; set; }
    
    public Guid Id { get; set; }
}