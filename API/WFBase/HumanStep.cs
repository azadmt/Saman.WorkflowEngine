namespace API.WFBase
{
    public abstract class HumanStep : IWorkflowStep
    {
        public abstract string Name { get; }
        public abstract string Role { get; }


        public async Task ExecuteAsync(WorkflowContext context, IServiceProvider sp)
        {
            context.SetData("WaitingRole", Role);
         
        }


        public abstract Task CompleteAsync(WorkflowContext context, IServiceProvider sp);
    }
}
