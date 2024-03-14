namespace ListMasterService.Models;

public class RegistrationResponse
{
    public string Token { get; set; }
    
    public string ExpiresAt { get; set; }
}