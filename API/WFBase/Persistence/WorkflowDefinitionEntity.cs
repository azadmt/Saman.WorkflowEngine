namespace API.WFBase.Persistence
{
    public class WorkflowDefinitionEntity
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public int Version { get; set; }
        public string DefinitionJson { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
