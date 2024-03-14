namespace ListMasterService.Models;

public class StatusCode
{
    public StatusCode(int code, string message)
    {
        Code = code;
        Message = message;
    }

    public int Code { get; set; }
    public string Message { get; set; }
    
    
}