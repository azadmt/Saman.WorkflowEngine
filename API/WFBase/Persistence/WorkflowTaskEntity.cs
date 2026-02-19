namespace API.WFBase.Persistence
{
    public class WorkflowTaskEntity
    {
        public Guid Id { get; set; }
        public Guid WorkflowInstanceId { get; set; }
        public string Role { get; set; }
        public WorkflowTaskStatus Status { get; set; }
        public string PayloadJson { get; set; }
    }
}