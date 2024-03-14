namespace ListMasterService.Models;

public class SignInResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Token { get; set; }
    
    public string ExpiresAt { get; set; }
}