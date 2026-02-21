namespace WorkflowBase;
public class TransitionDefinition
{
    public string Title { get; set; }
    public string? Event { get; set; }
    public string To { get; set; } = default!;
    public string ConditionExpression { get; set; }
    public Func<WorkflowContext, bool> ConditionFunc { get; set; }
}
