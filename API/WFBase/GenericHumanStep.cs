namespace API.WFBase
{
    public class GenericHumanStep : HumanStep
    {
        private readonly string _name;
        private readonly string _role;


        public GenericHumanStep(string name, string role)
        {
            _name = name;
            _role = role;
        }


        public override string Name => _name;
        public override string Role => _role;


        public override Task CompleteAsync(WorkflowContext context, IServiceProvider sp)
        {
            // UI داده‌ها را در Context ست می‌کند
            return Task.CompletedTask;
        }
    }
}
