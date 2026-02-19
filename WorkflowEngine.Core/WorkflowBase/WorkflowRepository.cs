using LiteDB;
using Newtonsoft.Json;

namespace WorkflowBase;



public interface IWorkflowRepository
{
    WorkflowInstance Get(Guid instanceId);
    void Save(WorkflowInstance instance);
}
public class LiteDbWorkflowRepository : IWorkflowRepository
{
    private readonly LiteDatabase _dbContext;

    public LiteDbWorkflowRepository(LiteDatabase liteDatabase)
    {
        _dbContext = liteDatabase;
    }

    public WorkflowInstance Get(Guid instanceId)
    {
        var instance = _dbContext.GetCollection<WorkflowInstance>()
            .Find(x=> x.Id==instanceId)
            .SingleOrDefault();

        return instance;
    }

    public void Save(WorkflowInstance instance)
    {
        _dbContext.GetCollection<WorkflowInstance>()
            .Upsert(instance);
    }
}

public class WorkflowRepository: IWorkflowRepository
{
    private Dictionary<Guid, string> _db = new();

    public WorkflowInstance Get(Guid instanceId)
    {
        var instance = _db[instanceId];

        return JsonConvert.DeserializeObject<WorkflowInstance>(instance);
    }

    public void Save(WorkflowInstance instance)
    {
        var json = JsonConvert.SerializeObject(instance);
        if (!_db.ContainsKey(instance.Id))
            _db.Add(instance.Id, json);
        else
            _db[instance.Id] = json;
    }
}
