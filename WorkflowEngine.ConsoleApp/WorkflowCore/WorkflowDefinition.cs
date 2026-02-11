public class WorkflowDefinition
{
    public string Name { get; init; } 
    public int Version { get; init; } = 1;
    public string StartState { get; init; }
    public Dictionary<string, StateDefinition> States { get; set; } = new();

    public string GetId()
    {
        return $"{Name}-V{Version}";
    }

}

public interface IWorkflowDefinition
{
    WorkflowDefinition GetDefinition();
}
