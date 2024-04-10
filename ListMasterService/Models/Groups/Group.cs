namespace ListMasterService.Models.Groups;

public class Group
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public Guid OwnerId { get; set; }
}