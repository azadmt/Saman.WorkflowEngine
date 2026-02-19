using RuleEngine.Base;
using WorkflowBase;

namespace WorkflowEngine.ConsoleApp.WorkflowDefinitions.HealthInsuranceIssueWorkflow;

public class EvaluateMedicalRiskActivity : IWorkflowActivity
{

    public async Task ExecuteAsync(WorkflowContext context)
    {
        var ruleSet = RuleSetRegistry.GetRuleSet(context.CurrentStateDefinition.RulesetId);

        var ruleresult = await ruleSet.EvaluateAsync(context);
        //var isSmoker = context.GetData<bool>("Smoker");
        //var hasSergrry = context.GetData<bool>("Sergery");

        var riskLevel = "low";
        if (ruleresult.HasBlockers)
            riskLevel = "high";

        if ((!ruleresult.HasBlockers && ruleresult.HasWarnings))
            riskLevel = "medium";

        context.SetData("RiskLevel", riskLevel);
    }
}

public class DoctorReviewActivity : IWorkflowActivity
{

    public async Task ExecuteAsync(WorkflowContext context)
    {
        Console.WriteLine("call DoctorReviewActivity");
    }
}

public class ApproveProposalActivity : IWorkflowActivity
{
    //   public override string Name => this.GetType().FullName;

    public async Task ExecuteAsync(WorkflowContext context)
    {
        Console.WriteLine("call ApproveProposalActivity");
    }
}

public class RejectProposalActivity : IWorkflowActivity
{
    //   public override string Name => this.GetType().FullName;

    public async Task ExecuteAsync(WorkflowContext context)
    {
        Console.WriteLine("call RejectProposalActivity");
    }
}

public class UserCompleteDocActivity : IWorkflowActivity
{
    //   public override string Name => this.GetType().FullName;

    public async Task ExecuteAsync(WorkflowContext context)
    {
        Console.WriteLine("call UserCompleteDocActivity");
    }
}