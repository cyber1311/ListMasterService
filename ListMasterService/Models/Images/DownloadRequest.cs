namespace ListMasterService.Models.Images;

public class DownloadRequest
{
    public Guid UserId { get; set; }
    public string ImageName { get; set; }
}