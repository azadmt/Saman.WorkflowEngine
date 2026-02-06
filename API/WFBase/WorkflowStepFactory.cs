namespace API.WFBase;

public static class WorkflowStepFactory
{
    public static IWorkflowStep Create(WorkflowStepDefinition def)
    {
        return def.Type switch
        {
            WorkflowStepType.Human => new GenericHumanStep(def.Name, def.Role!),
            WorkflowStepType.System => new GenericSystemStep(def.Name, ""),
            _ => throw new NotSupportedException(def.Type.ToString())
        };
    }
}
