using System.Text.Json;
using WorkflowCore;

public class WorkflowInstance
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string WorkflowDefinitionId { get; set; } = default!;

    public string CurrentStateName { get; set; } = default!;
    public WorkflowInstanceStatus Status { get; set; } = WorkflowInstanceStatus.Running;

    public WorkflowContext Context { get; set; } = new();

    public List<WorkflowHistoryEntry> WorkflowHistories { get; set; }=new();

    public void AddHistoryEntry()
    {
        var user = Context.CurrentStateDefinition.Type == StateType.HumanTask
            ? Context.CurrentStateDefinition.HumanTask?.User
            : "Auto";
        WorkflowHistories.Add(new WorkflowHistoryEntry { 
            WorkflowInstanceId=Id,
            StateName=CurrentStateName,
            User= user

        });
    }
}
