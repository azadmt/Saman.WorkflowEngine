using LiteDB;

namespace WorkflowBase;

public class WorkflowTaskService
{
    private readonly LiteDatabase _db;
    

    public WorkflowTaskService(LiteDatabase liteDatabase)
    {
        _db = liteDatabase;
    }
    public void Add(WorkflowTask workflowTaskEntity)
    {
        _db.GetCollection<WorkflowTask>().Insert(workflowTaskEntity);
    }

    public List<WorkflowTask> GetAvailableTasks(string role, string userName)
    {
        return _db
               .GetCollection<WorkflowTask>()
               .Find(x => x.Status == WorkflowTaskStatus.Open && (x.Role == role || x.Assignee == userName))
               .ToList();
    }

    public void AssigneTask(Guid taskId, string userRole, string userName)
    {
        var task = _db
               .GetCollection<WorkflowTask>()
               .Find(x => x.Id == taskId)
               .Single()
               ;

        if (userRole != task.Role)
            throw new UnauthorizedAccessException($"{userName} can't do this task");

        task.Assignee = userName;
    }

    public void CompleteTask(Guid taskId)
    {
        var task = _db
               .GetCollection<WorkflowTask>()
               .Find(x => x.Id == taskId)
               .Single()
               ;
        task.Status = WorkflowTaskStatus.Completed;
        task.CompleteTime = DateTime.Now;
        _db
          .GetCollection<WorkflowTask>()
          .Update(task);
    }
}


public interface IDbContext
{
    T Set<T>();
   
}