namespace WorkflowEngine.ConsoleApp.RuleEngineCore;

public interface IRule
{
    string Name { get; }
    string Title { get; }
    RuleSeverity Severity { get; } // Blocker, Warning, Info
    Task<RuleResult> EvaluateAsync(IRuleContext context, RuleParameters parameters);
}
