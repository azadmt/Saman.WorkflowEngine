using API.WFBase;

namespace API.Sample
{
    public class UnderwritingStep : HumanStep
    {
        public override string Name => "Underwriting";
        public override string Role => "CarExpert";


        public override Task CompleteAsync(WorkflowContext context, IServiceProvider sp)
        {
            context.SetData("ExtraRate", 0.2m);
            return Task.CompletedTask;
        }
    }


    public class ApplyRateStep : IWorkflowStep
    {
        public string Name => "ApplyRate";


        public async Task ExecuteAsync(WorkflowContext context, IServiceProvider sp)
        {
            var basePrice = context.GetData<decimal>("BasePrice");
            var rate = context.GetData<decimal>("ExtraRate");
            context.SetData("FinalPrice", basePrice + (basePrice * rate));


            var hook = sp.GetRequiredService<IWorkflowHook>();
            await hook.ExecuteAsync(context);
        }
    }
}
