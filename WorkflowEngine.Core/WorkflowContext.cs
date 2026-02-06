using System.Text.Json;
using System.Text.Json.Serialization;

namespace WorkflowEngine.Core
{
    // 2. Define the WorkflowContext class
    public class WorkflowContext
    {
        [JsonInclude]
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
    }
}
