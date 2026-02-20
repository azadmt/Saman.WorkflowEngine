using WorkflowBase;

namespace WrokflowDefinition.HealthInsuranceIssue.Activity;

public class DoctorReviewActivity : IWorkflowActivity
{

    public async Task ExecuteAsync(WorkflowContext context)
    {
        Console.WriteLine("call DoctorReviewActivity");
    }
}
