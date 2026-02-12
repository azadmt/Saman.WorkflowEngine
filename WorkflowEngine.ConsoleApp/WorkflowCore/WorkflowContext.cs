using Newtonsoft.Json;

public class WorkflowContext
{
    [Newtonsoft.Json.JsonProperty]
    public Dictionary<string, object> Data { get; private set; } = new Dictionary<string, object>();

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

    //public T Get<T>(string key) => Data.TryGetValue(key, out var v)
    //    ? JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(v))
    //    : default;
}

    //public class WorkflowContext
    //{
    //    public Dictionary<string, object> Data { get; set; } = new();


    //    public void Set<T>(string key, T value) => Data[key] = value;
    //    public T Get<T>(string key) => Data.TryGetValue(key, out var v)
    //    ? JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(v))
    //    : default;
    //}
//}
