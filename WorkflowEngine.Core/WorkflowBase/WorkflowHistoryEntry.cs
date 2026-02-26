namespace WorkflowBase;

public class WorkflowHistoryEntry
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid WorkflowInstanceId { get; set; }
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.Now;
    public string EventType { get; set; } // StateEntered, RuleEvaluated, ActivityExecuted :Enum
    public string StateName { get; set; }
    public string ActivityName { get; set; }
    public string? User { get; set; }
    public string? Data { get; set; } // JSON

    
}