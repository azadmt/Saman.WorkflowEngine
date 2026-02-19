namespace WorkflowBase;
public class StateDefinition
{
    public string Name { get; set; }
    public string RulesetId { get; set; } 
    public StateType Type { get; set; }
    public List<IWorkflowActivity> Activities { get; set; } = new();
    public List<TransitionDefinition> Transitions { get; set; } = new();
    public HumanTaskDefinition? HumanTask { get; set; }
}
