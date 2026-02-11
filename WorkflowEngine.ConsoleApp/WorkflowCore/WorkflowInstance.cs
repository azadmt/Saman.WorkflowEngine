using System.Text.Json;

public class WorkflowInstance
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string WorkflowDefinitionId { get; set; } = default!;
    //public int WorkflowVersion { get; set; }
    public string CurrentStateId { get; set; } = default!;
    public WorkflowInstanceStatus Status { get; set; } = WorkflowInstanceStatus.Running;
  //  public Dictionary<string, JsonElement> Variables { get; set; } = new();
    public WorkflowContext Context { get; set; } = new();
}
