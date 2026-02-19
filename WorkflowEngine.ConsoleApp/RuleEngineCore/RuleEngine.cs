using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowEngine.ConsoleApp.RuleEngineCore;

public static class RuleSetRegistry
{
    private static readonly Dictionary<string, RuleSet> _ruleSets = new();

    public static void RegisterRuleSet(RuleSet ruleSet)
    {
        _ruleSets[ruleSet.Id] = ruleSet;
    }

    public static RuleSet GetRuleSet(string ruleSetId) 
    {
        if (_ruleSets.TryGetValue(ruleSetId, out var ruleSet))
            return ruleSet;
        return null;
    }
}


public class RuleResult
{
    public string Name { get; set; }
    public bool Passed { get; set; }
    public RuleSeverity Severity { get; set; }
    public List<string> Messages { get; set; } = new();

}

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

public class RuleEvaluationResult
{
    public bool AllPassed { get; set; }
    public bool HasBlockers { get; set; }
    public bool HasWarnings { get; set; }
    public List<RuleResult> Results { get; set; }
}

public class RuleBinding
{
    public IRule Rule { get; set; }
    public RuleParameters Parameters { get; set; } = new();

    public int Priority { get; set; } = 0;
}