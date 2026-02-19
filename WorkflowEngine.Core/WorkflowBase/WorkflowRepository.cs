using Newtonsoft.Json;

namespace WorkflowBase;

public class WorkflowRepository
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
