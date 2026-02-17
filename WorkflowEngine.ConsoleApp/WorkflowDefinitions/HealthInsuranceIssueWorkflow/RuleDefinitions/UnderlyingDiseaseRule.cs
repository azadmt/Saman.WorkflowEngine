using WorkflowEngine.ConsoleApp.RuleEngineCore;
using WorkflowEngine.ConsoleApp.WorkflowDefinitions.HealthInsuranceIssueWorkflow.DataContract;

namespace WorkflowEngine.ConsoleApp.WorkflowDefinitions.HealthInsuranceIssueWorkflow.RuleDefinitions;

public class UnderlyingDiseaseRule : IRule
{
    public string Name => nameof(UnderlyingDiseaseRule);

    public string Title => "بیمه شدگان بیماری زمینه ای نداشته باشند";

    public RuleSeverity Severity => RuleSeverity.Blocker;

    public async Task<RuleResult> EvaluateAsync(IRuleContext context, RuleParameters parameters)
    {
        var policy = context.GetData<HealthPolicyRequest>("PolicyRequest");

        var invalidInsureds = policy.Insureds.Where(x => x.HasUnderlyingDisease);

        return new RuleResult()
        {
            Passed = !invalidInsureds.Any(),
            Messages = invalidInsureds.Select(x => $"{x.Id} ").ToList(),
        };

    }
}
