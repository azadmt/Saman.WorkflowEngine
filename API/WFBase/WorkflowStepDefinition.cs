namespace API.WFBase;

public enum WorkflowStepType
{
    System,
    Human,
    Event
}

public class WorkflowStepDefinition
{
    public WorkflowStepType Type { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? Role { get; set; }
    public string ApiEndpoint { get; set; }
    public string EventName { get; set; }
}
