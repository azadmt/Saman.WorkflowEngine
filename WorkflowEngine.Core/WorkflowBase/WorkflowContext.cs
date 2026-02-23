using Newtonsoft.Json;
using RuleEngine.Base;
using System.Text.Json;
namespace WorkflowBase;
public class WorkflowContext: IWorkflowContex, IRuleContext//؟؟
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

  
        if (value is T typedValue)
            return typedValue;

  
        if (value is Newtonsoft.Json.Linq.JToken jToken)
        {
            return jToken.ToObject<T>();
        }

        if (value is JsonElement element)
            return element.Deserialize<T>();


        try
        {
            return (T)value;
        }
        catch
        {
            return default;
        }
    }

        [JsonIgnore]
    public StateDefinition CurrentStateDefinition { get; set; }

    public IReadOnlyList<TransitionDefinition> GetTransitions()
    {
        return CurrentStateDefinition.Transitions;
    }
}
