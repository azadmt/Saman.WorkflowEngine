namespace WorkflowCore;

public class WorkflowTaskService
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

    public void AssigneTask(Guid taskId, string userRole, string userName)
    {
        var task= _db
               .Single(x => x.Id==taskId)
               ;

        if (userRole != task.Role)
            throw new  UnauthorizedAccessException($"{userName} can't do this task");

        task.Assignee= userName;
    }
}

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