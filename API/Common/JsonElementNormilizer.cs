using System.Collections;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;


public class WorkflowStartRequest
{
    public string WorkflowDefinitionName { get; set; }
    public int WorkflowDefinitionVersion { get; set; }
    public Dictionary<string, JsonElement> Input { get; set; } = new();
}