namespace ListMasterService.Models.Images;

public class UploadRequest
{
    public Guid UserId { get; set; }
    public string ImageName { get; set; }
}