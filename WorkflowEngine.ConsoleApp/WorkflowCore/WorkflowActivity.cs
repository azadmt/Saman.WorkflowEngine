public abstract class WorkflowActivity
{
    public virtual string Name => this.GetType().FullName;

    public abstract Task ExecuteAsync(WorkflowContext context);
}
