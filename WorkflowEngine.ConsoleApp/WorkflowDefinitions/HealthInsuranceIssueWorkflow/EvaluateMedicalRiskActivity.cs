using System;

namespace WorkflowEngine.ConsoleApp.WorkflowDefinitions.HealthInsuranceIssueWorkflow;

public class EvaluateMedicalRiskActivity : WorkflowActivity
{
    //   public override string Name => this.GetType().FullName;

    public override async Task ExecuteAsync(WorkflowContext context)
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

public class DoctorReviewActivity : WorkflowActivity
{
    //   public override string Name => this.GetType().FullName;

    public override async Task ExecuteAsync(WorkflowContext context)
    {
        Console.WriteLine("call DoctorReviewActivity");
    }
}

public class ApproveProposalActivity : WorkflowActivity
{
    //   public override string Name => this.GetType().FullName;

    public override async Task ExecuteAsync(WorkflowContext context)
    {
        Console.WriteLine("call ApproveProposalActivity");
    }
}

public class RejectProposalActivity : WorkflowActivity
{
    //   public override string Name => this.GetType().FullName;

    public override async Task ExecuteAsync(WorkflowContext context)
    {
        Console.WriteLine("call RejectProposalActivity");
    }
}

public class UserCompleteDocActivity : WorkflowActivity
{
    //   public override string Name => this.GetType().FullName;

    public override async Task ExecuteAsync(WorkflowContext context)
    {
        Console.WriteLine("call UserCompleteDocActivity");
    }
}