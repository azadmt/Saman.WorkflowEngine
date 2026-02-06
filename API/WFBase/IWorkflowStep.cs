namespace API.WFBase
{
    public interface IWorkflowStep
    {
        string Name { get; }
        Task ExecuteAsync(WorkflowContext context, IServiceProvider sp);
    }
}
