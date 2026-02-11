namespace API.WFBase;

public class WorkflowDefinition
{
    public string Name { get; set; }
    public int Version { get; set; }
    public List<WorkflowStepDefinition> Steps { get; set; }
}
