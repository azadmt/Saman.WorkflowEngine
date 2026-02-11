public class StateDefinition
{
    public string Id { get; set; } = default!;
    public StateType Type { get; set; }
    public List<WorkflowActivity> Activities { get; set; } = new();
    public List<TransitionDefinition> Transitions { get; set; } = new();
    public HumanTaskDefinition? HumanTask { get; set; }
}
