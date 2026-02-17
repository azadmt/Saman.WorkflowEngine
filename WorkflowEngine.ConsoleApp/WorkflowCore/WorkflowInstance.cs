using System.Text.Json;
using WorkflowCore;

public class WorkflowInstance
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string WorkflowDefinitionId { get; set; } = default!;

    public string CurrentStateName { get; set; } = default!;
    public WorkflowInstanceStatus Status { get; set; } = WorkflowInstanceStatus.Running;

    public WorkflowContext Context { get; set; } = new();

    public List<WorkflowHistoryEntry> workflowHistories { get; set; }=new();
}
