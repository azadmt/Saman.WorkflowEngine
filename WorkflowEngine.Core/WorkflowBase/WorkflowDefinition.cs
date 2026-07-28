namespace WorkflowBase;
public class WorkflowDefinition
{
    public string Name { get; init; } 
    public int Version { get; init; } = 1;
    public string StartState { get; set; }
    public string RuleSetId { get; set; }//??
    public Dictionary<string, StateDefinition> States { get; set; } = new();

    public string GetId()
    {
        return $"{Name}-V{Version}";
    }

}

public interface IWorkflowDefinitionFactory
{
    WorkflowDefinition GetDefinition();
}
