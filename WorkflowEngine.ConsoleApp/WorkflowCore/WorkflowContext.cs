using Newtonsoft.Json;

public class WorkflowContext: IWorkflowContex, IRuleContext
{
    [JsonProperty]
    public Dictionary<string, object> Data { get; private set; } = new Dictionary<string, object>();

    [JsonIgnore]
    public Dictionary<string, object> LocalInput { get; private set; } = new Dictionary<string, object>();


    public void SetData(string key, object value)
    {
        Data[key] = value;
    }

    public T? GetData<T>(string key)
    {
        if (!Data.TryGetValue(key, out var value))
            return default;

        return (T)value;
    }

    [JsonIgnore]
    public StateDefinition CurrentStateDefinition { get; set; }

    public IReadOnlyList<TransitionDefinition> GetTransitions()
    {
        return CurrentStateDefinition.Transitions;
    }
}

public interface IDataContext
{
    T? GetData<T>(string key);
    void SetData(string key, object value);
}

public interface IRuleContext : IDataContext { }
public interface IWorkflowContex:IDataContext
{
    IReadOnlyList<TransitionDefinition> GetTransitions();
}
