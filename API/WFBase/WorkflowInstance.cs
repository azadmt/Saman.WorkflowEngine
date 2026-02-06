namespace API.WFBase
{
    public class WorkflowInstance
    {
        public Guid Id { get; set; }
        public string WorkflowName { get; set; } = default!;
        public WorkflowStatus Status { get; set; }
        public int CurrentStepIndex { get; set; }
        public string ContextJson { get; set; } = default!;
    }
}
