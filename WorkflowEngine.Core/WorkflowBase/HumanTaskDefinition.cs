using LiteDB;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text.Json.Serialization;

namespace WorkflowBase;
public class HumanTaskDefinition
{
    public Func<WorkflowContext, string> AutoAssigne { get; set; }
    public string User { get; set; } = default!;
    public string Role { get; set; } = default!;
    public string UiContract { get; set; } = default!;
    public string Title { get; set; } = default!;
    public List<TaskInput> Inputs { get; set; } = new();
    public List<DisplayField> ContextDisplayFields { get; set; } = new();
    public string? ContextHtmlTemplate { get; set; }
}

public class DisplayField
{
    public string Label { get; set; }
    public object Value { get; set; }
    public int Order { get; set; }
    public string? Format { get; set; }
    public string? CssClass { get; set; }
    [BsonIgnore]
    [JsonIgnore]
    public Func<WorkflowContext, object>? ValueProvider { get; init; }
}