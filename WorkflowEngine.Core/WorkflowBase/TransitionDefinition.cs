namespace WorkflowBase;
public class TransitionDefinition
{
    public string? Event { get; set; }
    public string To { get; set; } = default!;
    public Func<WorkflowContext, bool> Condition { get; set; }
}
