using System.Net.Http.Json;
using WorkflowBase;
using WrokflowDefinition.HealthInsuranceIssue.DataContract;

namespace WrokflowDefinition.HealthInsuranceIssue.Activity;

public class ApproveProposalActivity : IWorkflowActivity
{
    //   public override string Name => this.GetType().FullName;
    public ApproveProposalActivity()
    {
            
    }

    public async Task ExecuteAsync(WorkflowContext context)
    {
        var policy = context.GetData<HealthPolicyRequest>("PolicyRequest");
        var client = new HttpClient();
        client.BaseAddress = new Uri("https://localhost:7106/");

        var request = new { PolicyId= policy.Id, ExtraRate= policy.ExtraRate };

        await client.PostAsJsonAsync("api/Policy/approve", request);
    }
}
