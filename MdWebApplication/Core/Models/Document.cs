namespace Core.Models;

public class Document
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    /// <summary>
    /// URL документа в MinIO
    /// </summary>
    public string FileUrl { get; set; }
    
    public string FileName { get; set; }
    
    public bool IsSharing { get; set; }
}