using System;

namespace WorkflowEngine.ConsoleApp.WorkflowDefinitions.HealthInsuranceIssueWorkflow;

public class EvaluateMedicalRiskActivity : WorkflowActivity
{
    //   public override string Name => this.GetType().FullName;

    public override async Task ExecuteAsync(WorkflowContext context)
    {
        var isSmoker = context.GetData<bool>("Smoker");

        Console.WriteLine("call EvaluateMedicalRiskActivity");
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