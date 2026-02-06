namespace API.WFBase;

    public class WorkflowDefinition
    {
        public string Name { get; set; } = default!;
        public List<WorkflowStepDefinition> Steps { get; set; } = new();
    }

