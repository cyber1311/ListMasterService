namespace ListMasterService.Models.Groups;

public class GroupUpdateTitleRequest
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public Guid UserId { get; set; }
}