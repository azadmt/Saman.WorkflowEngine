using WorkflowBase;

namespace WrokflowDefinition.HealthInsuranceIssue.Activity;

public class RejectProposalActivity : IWorkflowActivity
{
    //   public override string Name => this.GetType().FullName;

    public async Task ExecuteAsync(WorkflowContext context)
    {
        Console.WriteLine("call RejectProposalActivity");
    }
}
