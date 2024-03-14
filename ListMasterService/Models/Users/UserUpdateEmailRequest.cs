namespace ListMasterService.Models;

public class UserUpdateEmailRequest
{
    public Guid Id { get; set; }
    public string Email { get; set; }
}