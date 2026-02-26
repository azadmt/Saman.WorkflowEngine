namespace WorkflowBase;
public class WorkflowTask
{
    public Guid Id { get; set; }= Guid.NewGuid();
    public Guid WorkflowInstanceId { get; set; }
    public string WorkflowDefinitionId { get; set; }
    public string Title { get; set; }
    public string Role { get; set; }
    public WorkflowTaskStatus Status { get; set; }
    public string Assignee { get; set; }
    public DateTimeOffset? CompletedOn{ get; set; }
    public DateTimeOffset CreatedOn{ get; set; }=DateTimeOffset.Now;
    public List<TaskInput> Inputs { get; set; } = new();  
    public List<DisplayField> ContextDisplayFields { get; set; } = new();
}
