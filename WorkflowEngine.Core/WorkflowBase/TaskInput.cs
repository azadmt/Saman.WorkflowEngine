using LiteDB;
using System.Text.Json.Serialization;

namespace WorkflowBase;

public class TaskInput
{
    public string Name { get; set; }
    public string Lable { get; set; }
    public InputType Type { get; set; }
    public bool IsRequired{ get; set; }
    public List<KeyValuePair<string, string>> Options { get; set; } = new();
    [BsonIgnore]
    [JsonIgnore]
    public Func<IWorkflowContex, List<KeyValuePair<string, string>>> OptionsDataProvider { get; set; } 

}

