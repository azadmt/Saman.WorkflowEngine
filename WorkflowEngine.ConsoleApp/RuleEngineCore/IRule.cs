namespace WorkflowEngine.ConsoleApp.RuleEngineCore;

public interface IRule
{
    string Name { get; }
    string Description { get; }
    RuleSeverity Severity { get; } // Blocker, Warning, Info
    Task<RuleResult> EvaluateAsync(WorkflowContext context, RuleParameters parameters);
}
