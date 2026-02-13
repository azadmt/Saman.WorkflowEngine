using System;

namespace WorkflowEngine.ConsoleApp.WorkflowDefinitions.HealthInsuranceIssueWorkflow;

public class EvaluateMedicalRiskActivity : IWorkflowActivity
{
    private readonly HttpClient _httpClient;

    public EvaluateMedicalRiskActivity(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    public  async Task ExecuteAsync(WorkflowContext context)
    {
        var isSmoker = context.GetData<bool>("Smoker");
        var hasSergrry = context.GetData<bool>("Sergery");

        var riskLevel = "low";
        if ((isSmoker & hasSergrry))
              riskLevel = "high";

        if ((isSmoker || hasSergrry))
            riskLevel = "medium";

        context.SetData("RiskLevel", riskLevel);
    }
}

public class DoctorReviewActivity : IWorkflowActivity
{

    public  async Task ExecuteAsync(WorkflowContext context)
    {
        Console.WriteLine("call DoctorReviewActivity");
    }
}

public class ApproveProposalActivity : IWorkflowActivity
{
    //   public override string Name => this.GetType().FullName;

    public  async Task ExecuteAsync(WorkflowContext context)
    {
        Console.WriteLine("call ApproveProposalActivity");
    }
}

public class RejectProposalActivity : IWorkflowActivity
{
    //   public override string Name => this.GetType().FullName;

    public  async Task ExecuteAsync(WorkflowContext context)
    {
        Console.WriteLine("call RejectProposalActivity");
    }
}

public class UserCompleteDocActivity : IWorkflowActivity
{
    //   public override string Name => this.GetType().FullName;

    public  async Task ExecuteAsync(WorkflowContext context)
    {
        Console.WriteLine("call UserCompleteDocActivity");
    }
}