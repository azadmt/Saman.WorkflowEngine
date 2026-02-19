namespace API.WFBase.Persistence
{
    public class WorkflowInstanceEntity
    {
        public Guid Id { get; set; }
        public Guid DefinitionId { get; set; }
        public int CurrentStepIndex { get; set; }
        public WorkflowStatus Status { get; set; }
        public string ContextJson { get; set; }
    }
}
