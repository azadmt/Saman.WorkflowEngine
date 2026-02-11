using Newtonsoft.Json;

namespace WorkflowCore;

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
        if (!_db.ContainsKey(instance.Id))
            _db.Add(instance.Id, JsonConvert.SerializeObject(instance));
        else
            _db[instance.Id] = JsonConvert.SerializeObject(instance);
    }
}
