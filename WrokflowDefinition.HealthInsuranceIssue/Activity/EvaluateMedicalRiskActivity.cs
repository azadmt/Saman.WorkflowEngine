using RuleEngine.Base;
using WorkflowBase;

namespace WrokflowDefinition.HealthInsuranceIssue.Activity;

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

        if (!ruleresult.HasBlockers && ruleresult.HasWarnings)
            riskLevel = "medium";

        context.SetData("RiskLevel", riskLevel);//Save To DB
    }
}
