using API.WFBase;

namespace API.Sample.InsuranceUnderwriting;

public class ApplyRateStep : IWorkflowStep
{
    public string Name => "ApplyRate";


    public async Task ExecuteAsync(WorkflowContext context, IServiceProvider sp)
    {
        var basePrice = context.GetData<decimal>("BasePrice");
        var rate = context.GetData<decimal>("ExtraRate");
        context.SetData("FinalPrice", basePrice + basePrice * rate);


        var hook = sp.GetRequiredService<IWorkflowHook>();
        await hook.ExecuteAsync(context);
    }
}
