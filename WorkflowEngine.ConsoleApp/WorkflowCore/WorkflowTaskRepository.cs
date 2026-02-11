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