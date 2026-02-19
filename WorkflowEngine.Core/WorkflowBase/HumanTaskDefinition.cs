namespace WorkflowBase;
public class HumanTaskDefinition
{
    public Func<WorkflowContext, string> AutoAssigne { get; set; }
    public string User { get; set; } = default!;
    public string Role { get; set; } = default!;
    public string UiContract { get; set; } = default!;
    public List<TaskInput> Inputs{ get; set; } = new();
}

