namespace WorkflowBase;
public class WorkflowTask
{
    public Guid Id { get; set; }= Guid.NewGuid();
    public Guid WorkflowInstanceId { get; set; }
    public string Role { get; set; }
    public WorkflowTaskStatus Status { get; set; }
    public string Assignee { get; set; }
    public DateTimeOffset CompletedOn{ get; set; }
    public DateTimeOffset CreatedOn{ get; set; }
    public List<TaskInput> Inputs { get; set; } = new();  
    public List<DisplayField> ContextDisplayFields { get; set; } = new();
}
