using API.WFBase;

namespace API.Sample.InsuranceUnderwriting;

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
