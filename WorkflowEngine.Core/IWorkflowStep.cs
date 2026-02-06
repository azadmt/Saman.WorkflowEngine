namespace WorkflowEngine.Core
{
    // 1. Define the IWorkflowStep interface
    public interface IWorkflowStep
    {
        string Name { get; }
        Task ExecuteAsync(WorkflowContext context);
    }
}
