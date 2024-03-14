namespace ListMasterService.Models;

public class UserUpdatePasswordRequest
{
    public Guid Id { get; set; }
    public string Password { get; set; }
}