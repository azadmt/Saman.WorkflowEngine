namespace WorkflowCore;

public class WorkflowTaskRepository
{
    private List<WorkflowTask> _db = new List<WorkflowTask>();
    public void Add(WorkflowTask workflowTaskEntity)
    {
        _db.Add(workflowTaskEntity);
    }

    public List<WorkflowTask> GetAvailableTasks(string role, string userName)
    {
        return _db
               .Where(x => x.Status == WorkflowTaskStatus.Open && (x.Role == role || x.Assignee == userName))
               .ToList();
    }
}

public class WorkflowHistoryEntry
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid WorkflowInstanceId { get; set; }
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.Now;
    public string EventType { get; set; } // StateEntered, RuleEvaluated, ActivityExecuted
    public string StateName { get; set; }
    public string ActivityName { get; set; }
    public string? User { get; set; }
    public string? Data { get; set; } // JSON
}