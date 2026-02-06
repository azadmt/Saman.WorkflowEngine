namespace API.WFBase;

public static class WorkflowStepFactory
{
    public static IWorkflowStep Create(WorkflowStepDefinition def)
    {
        return def.Type switch
        {
            WorkflowStepType.Human => new GenericHumanStep(def.Name, def.Role!),
            WorkflowStepType.System => new GenericSystemStep(def.Name, def.Hook),
            _ => throw new NotSupportedException(def.Type.ToString())
        };
    }
}

public class WorkflowDefinitionEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string DefinitionJson { get; set; } = default!;
}


public class WorkflowTask
{
    public Guid Id { get; set; }
    public Guid WorkflowInstanceId { get; set; }
    public string StepName { get; set; } = default!;
    public string Role { get; set; } = default!;
    public bool IsCompleted { get; set; }
}
