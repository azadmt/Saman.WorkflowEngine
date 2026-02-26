namespace RuleEngine.Base;
public class RuleSet
{
    public string Id { get; set; }
    public List<RuleBinding> Rules { get; set; } = new();

   
    public async Task<RuleEvaluationResult> EvaluateAsync(IRuleContext context)
    {
        var results = new List<RuleResult>();
        var blockers = 0;
        var warnings = 0;

        foreach (var binding in Rules.OrderBy(x=>x.Priority))
        {
            var result = await binding.Rule.EvaluateAsync(context, binding.Parameters);
            results.Add(result);

            if (!result.Passed)
            {
                if (result.Severity == RuleSeverity.Blocker) blockers++;
                else if (result.Severity == RuleSeverity.Warning) warnings++;
            }
        }

        return new RuleEvaluationResult
        {
            AllPassed = blockers == 0,
            HasBlockers = blockers > 0,
            HasWarnings = warnings > 0,
            Results = results
        };
    }
}
