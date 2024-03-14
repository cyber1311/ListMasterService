namespace ListMasterService.Models;

public class ListCreateRequest
{
    public Guid UserId { get; set; }
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Elements { get; set; }
}