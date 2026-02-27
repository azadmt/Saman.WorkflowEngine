namespace WorkflowBase;

public class WorkflowDocument
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
  
    public string RequestedInStateId { get; set; } = default!;
    public string RequestedInStateTitle { get; set; } = default!;
    public DateTimeOffset RequestedAt { get; set; } = DateTime.UtcNow;
  
    public string Label { get; set; } = default!;

    public string? FileUrl { get; set; }

    public DateTimeOffset? UploadedAt { get; set; }
    public string? UploadedByUserId { get; set; }

    public DocumentStatus Status { get; set; } = DocumentStatus.Pending;
    public string? ReviewComment { get; set; }  
    public DateTime? ReviewedAt { get; set; }
    public string? ReviewedByUserId { get; set; }
}

public enum DocumentStatus
{
    Pending ,     
    Uploaded ,    
    Approved ,    
    Rejected ,    
    NotRequired   
}