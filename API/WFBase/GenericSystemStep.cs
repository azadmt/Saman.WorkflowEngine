namespace API.WFBase
{
    public class GenericSystemStep : IWorkflowStep
    {
        private readonly string _name;
        private readonly string? _hook;


        public GenericSystemStep(string name, string? hook)
        {
            _name = name;
            _hook = hook;
        }


        public string Name => _name;


        public async Task ExecuteAsync(WorkflowContext context, IServiceProvider sp)
        {
            if (_hook == "PublishEvent")
            {
                var hook = sp.GetRequiredService<IWorkflowHook>();
                await hook.ExecuteAsync(context);
            }
        }
    }
}
