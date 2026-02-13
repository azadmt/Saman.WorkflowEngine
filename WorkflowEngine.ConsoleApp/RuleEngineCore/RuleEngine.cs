using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowEngine.ConsoleApp.RuleEngineCore;

public class RuleEngine
{
}

public enum RuleSeverity
{
    Blocker,   
    Warning,    
    Info        
}

public class RuleResult
{
    public bool Passed { get; set; }
    public RuleSeverity Severity { get; set; }
    public string Message { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
}

public class RuleSet
{
    public string Id { get; set; }
    public List<RuleBinding> Rules { get; set; } = new();

   
    public async Task<RuleEvaluationResult> EvaluateAsync(WorkflowContext context)
    {
        var results = new List<RuleResult>();
        var blockers = 0;
        var warnings = 0;

        foreach (var binding in Rules)
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
}