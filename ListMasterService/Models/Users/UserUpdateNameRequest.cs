namespace ListMasterService.Models;

public class UserUpdateNameRequest
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}