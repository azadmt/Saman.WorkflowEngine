using LiteDB;
using Newtonsoft.Json;
using WorkflowEngine.Core.Persistence;

namespace WorkflowBase;

public interface IWorkflowRepository
{
    WorkflowInstance Get(Guid instanceId);

    void Add(WorkflowInstance instance);
    void Update(WorkflowInstance instance);
}

public class LiteDbWorkflowRepository : IWorkflowRepository
{
    private readonly LiteDbContext _dbContext;

    public LiteDbWorkflowRepository(LiteDbContext liteDatabase)
    {
        _dbContext = liteDatabase;
    }

    public WorkflowInstance Get(Guid instanceId)
    {
        var instance = _dbContext.Set<WorkflowInstance>()
            .Find(x => x.Id == instanceId)
            .SingleOrDefault();

        return instance;
    }

    public void Add(WorkflowInstance instance)
    {
        _dbContext.Add(instance);
    }

    public void Update(WorkflowInstance instance)
    {
        _dbContext.Update(instance);
    }
}

public class WorkflowRepository : IWorkflowRepository
{
    private Dictionary<Guid, string> _db = new();

    public WorkflowInstance Get(Guid instanceId)
    {
        var instance = _db[instanceId];

        return JsonConvert.DeserializeObject<WorkflowInstance>(instance);
    }

    public void Add(WorkflowInstance instance)
    {
        var json = JsonConvert.SerializeObject(instance);
        if (!_db.ContainsKey(instance.Id))
            _db.Add(instance.Id, json);
        else
            _db[instance.Id] = json;
    }

    public void Update(WorkflowInstance instance)
    {
        throw new NotImplementedException();
    }
}