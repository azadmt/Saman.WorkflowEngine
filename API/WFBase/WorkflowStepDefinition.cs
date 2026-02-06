namespace API.WFBase
{
    public enum WorkflowStepType { Human,System }
    public class WorkflowStepDefinition
    {
        public WorkflowStepType Type { get; set; } = default!; // Human | System
        public string Name { get; set; } = default!;
        public string? Role { get; set; }
        public string? Hook { get; set; }
    }
}
