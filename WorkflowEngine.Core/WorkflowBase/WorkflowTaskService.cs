using LiteDB;
using System.Threading.Tasks;
using WorkflowEngine.Core.Persistence;

namespace WorkflowBase;

public class WorkflowTaskService
{
    private readonly LiteDbContext _db;

    public WorkflowTaskService(LiteDbContext liteDatabase)
    {
        _db = liteDatabase;
    }

    public void Add(WorkflowTask workflowTaskEntity)
    {
        _db.Add(workflowTaskEntity);
    }

    public List<WorkflowTask> GetAvailableTasks(string role, string userName)
    {
        var workflowTasks = _db
               .Set<WorkflowTask>()
               .Query()
               .Where(x => x.Status == WorkflowTaskStatus.Open && (x.Role == role || x.Assignee == userName))
               .ToList();

        //foreach (var item in workflowTasks.SelectMany(x=>x.ContextDisplayFields).ToList())
        //{
        //  var  workflowDefinition=WorkflowDefinitionRegistry.Get(item.WorkflowDefinitionId);
        //    workflowDefinition.States[""].HumanTask.ContextDisplayFields
        //}
        return workflowTasks;
    }

    public void AssigneTask(Guid taskId, string userRole, string userName)
    {
        var task = _db
               .Set<WorkflowTask>()
               .Find(x => x.Id == taskId)
               .Single()
               ;

        if (userRole != task.Role)
            throw new UnauthorizedAccessException($"{userName} can't do this task");

        task.Assignee = userName;
        _db.Update(task);
    }

    public WorkflowTask TaskDetail(Guid taskId)
    {
        return _db
               .Set<WorkflowTask>()
               .Find(x => x.Id == taskId)
               .Single()
               ;
    }

    public void CompleteTask(Guid taskId)
    {
        var task = _db
               .Set<WorkflowTask>()
               .Find(x => x.Id == taskId)
               .Single()
               ;
        task.Status = WorkflowTaskStatus.Completed;
        task.CompletedOn = DateTime.Now;
        _db.Update(task);
    }
}