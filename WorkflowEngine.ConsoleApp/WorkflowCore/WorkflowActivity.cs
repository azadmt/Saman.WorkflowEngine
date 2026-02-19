public interface IWorkflowActivity
{
    public virtual string Name => this.GetType().FullName;

    public abstract Task ExecuteAsync(WorkflowContext context);
}
