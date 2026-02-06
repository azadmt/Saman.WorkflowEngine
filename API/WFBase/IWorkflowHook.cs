namespace API.WFBase
{
    public interface IWorkflowHook
    {
        Task ExecuteAsync(WorkflowContext context);
    }
}
