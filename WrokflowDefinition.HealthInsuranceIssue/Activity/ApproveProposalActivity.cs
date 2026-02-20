using WorkflowBase;

namespace WrokflowDefinition.HealthInsuranceIssue.Activity;

public class ApproveProposalActivity : IWorkflowActivity
{
    //   public override string Name => this.GetType().FullName;

    public async Task ExecuteAsync(WorkflowContext context)
    {
        Console.WriteLine("call ApproveProposalActivity");
    }
}
